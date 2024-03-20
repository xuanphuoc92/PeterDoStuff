/*ref: https://learn.microsoft.com/en-us/aspnet/core/blazor/file-downloads?view=aspnetcore-6.0#download-from-a-stream*/
window.downloadFileFromStream = async (fileName, contentStreamReference) => {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
}

window.getVideoDeviceIds = async function () {
    const devices = await navigator.mediaDevices.enumerateDevices();
    const videoDevices = devices.filter(device => device.kind === 'videoinput');
    return videoDevices.map(device => device.deviceId);
}

window.startCameraStream = async function (videoElementId, deviceId) {
    let constraint;
    if (deviceId === "") {
        constraint = { video: true };
    }
    else {
        constraint = { video: { deviceId: { exact: deviceId } } };
    }
    const stream = await navigator.mediaDevices.getUserMedia(constraint);
    const videoElement = document.getElementById(videoElementId);
    videoElement.srcObject = stream;
}

window.stopCameraStream = function (videoElementId) {
    var videoElement = document.getElementById(videoElementId);
    var stream = videoElement.srcObject;
    var tracks = stream.getTracks();
    tracks.forEach(function (track) {
        track.stop();
    });
    videoElement.srcObject = null;
}

window.captureImage = function (videoElementId) {
    var videoElement = document.getElementById(videoElementId);
    var canvas = document.createElement('canvas');
    canvas.width = videoElement.videoWidth;
    canvas.height = videoElement.videoHeight;
    canvas.getContext('2d').drawImage(videoElement, 0, 0, canvas.width, canvas.height);
    return canvas.toDataURL('image/png');
};

window.getLocationCoordinates = function () {
    return new Promise((resolve, reject) => {
        navigator.geolocation.getCurrentPosition(
            function (position) {
                const location = {
                    latitude: position.coords.latitude,
                    longitude: position.coords.longitude
                };
                resolve(location);
            },
            function (error) {
                console.error('Error getting location:', error);
                reject(error); // Error occurred while getting location
            }
        );
    });
};

function loadMapWithMarker(mapDivId, latitude, longitude) {
    const mapOptions = {
        center: { lat: latitude, lng: longitude },
        zoom: 16
    };

    const mapDiv = document.getElementById(mapDivId);
    const map = new google.maps.Map(mapDiv, mapOptions);

    const marker = new google.maps.Marker({
        position: { lat: latitude, lng: longitude },
        map: map,
        title: 'Marker'
    });
}

window.keyDownFunction = (dotnetHelper, methodName) => {
    document.addEventListener('keydown', function (event) {
        dotnetHelper.invokeMethodAsync(methodName, event.key);
    });
};

window.swipeDetection = (dotnetHelper, methodName) => {
    let initialX = null;
    let initialY = null;

    window.addEventListener('touchstart', (event) => {
        initialX = event.touches[0].clientX;
        initialY = event.touches[0].clientY;
    }, { passive: false });

    window.addEventListener('touchmove', (event) => {
        if (!initialX || !initialY) {
            return;
        }

        const currentX = event.touches[0].clientX;
        const currentY = event.touches[0].clientY;
        const diffX = initialX - currentX;
        const diffY = initialY - currentY;

        if (Math.abs(diffX) > Math.abs(diffY)) {
            if (diffX > 0) {
                dotnetHelper.invokeMethodAsync(methodName, 'left');
            } else {
                dotnetHelper.invokeMethodAsync(methodName, 'right');
            }
        } else {
            if (diffY > 0) {
                dotnetHelper.invokeMethodAsync(methodName, 'up');
            } else {
                dotnetHelper.invokeMethodAsync(methodName, 'down');
            }
        }

        initialX = null;
        initialY = null;
    }, { passive: false });
};

window.decodeQRCode = function (imageId) {

    const img = document.getElementById(imageId);

    const canvas = document.createElement('canvas');
    const ctx = canvas.getContext('2d');

    // Set the canvas dimensions to match the image
    canvas.width = img.width;
    canvas.height = img.height;

    // Draw the image on the canvas
    ctx.drawImage(img, 0, 0);

    // Get the image data from the canvas
    const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height);

    // Decode QR code using jsQR
    const code = jsQR(imageData.data, imageData.width, imageData.height);

    // Return the decoded QR code result
    return code ? code.data : null;
};