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

window.openCameraStream = function (videoElementId) {
    return new Promise((resolve, reject) => {
        navigator.mediaDevices.getUserMedia({ video: true })
            .then(function (stream) {
                var videoElement = document.getElementById(videoElementId);
                videoElement.srcObject = stream;
                resolve(true); // Resolve the promise with true if stream is opened successfully
            })
            .catch(function (error) {
                console.error('Error accessing the camera:', error);
                reject(false); // Reject the promise with false if an error occurs
            });
    });
};

window.closeCameraStream = function (videoElementId) {
    // Set the video stream as the source of the video element
    var videoElement = document.getElementById(videoElementId);
    var stream = videoElement.srcObject;
    var tracks = stream.getTracks();

    tracks.forEach(function (track) {
        track.stop();
    });
    videoElement.srcObject = null;
};

window.captureImage = function (videoElementId) {
    return new Promise((resolve, reject) => {
        var videoElement = document.getElementById(videoElementId);
        var canvas = document.createElement('canvas');
        canvas.width = videoElement.videoWidth;
        canvas.height = videoElement.videoHeight;
        canvas.getContext('2d').drawImage(videoElement, 0, 0, canvas.width, canvas.height);
        var imageDataUrl = canvas.toDataURL('image/png');
        resolve(imageDataUrl);
    });
};

window.requestLocationPermission = function () {
    return new Promise((resolve, reject) => {
        if ('geolocation' in navigator) {
            navigator.permissions.query({ name: 'geolocation' })
                .then(function (permissionStatus) {
                    if (permissionStatus.state === 'granted') {
                        resolve(true); // Location permission already granted
                    } else {
                        navigator.geolocation.getCurrentPosition(
                            function (position) {
                                resolve(true); // Location permission granted
                            },
                            function (error) {
                                console.error('Error getting location:', error);
                                resolve(false); // Location permission denied or error occurred
                            }
                        );
                    }
                })
                .catch(function (error) {
                    console.error('Error querying location permission:', error);
                    resolve(false); // Location permission denied or error occurred
                });
        } else {
            console.error('Geolocation is not supported.');
            resolve(false); // Geolocation not supported
        }
    });
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