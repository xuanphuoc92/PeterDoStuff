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

window.copyToClipboard = function (text) {
    // Create a temporary textarea element
    var textarea = document.createElement("textarea");
    textarea.value = text;
    textarea.style.position = "fixed";  // Ensure it's not visible
    document.body.appendChild(textarea);

    // Select and copy the text
    textarea.select();
    document.execCommand("copy");

    // Clean up
    document.body.removeChild(textarea);
};

window.encryptAes = async (data, rawKey, iv) => {
    const key = await crypto.subtle.importKey(
        'raw',
        rawKey,
        { name: 'AES-CBC' },
        false,
        ['encrypt']
    );

    const encrypted = await window.crypto.subtle.encrypt(
        {
            name: "AES-CBC",
            iv
        },
        key,
        data
    );

    return new Uint8Array(encrypted);
}

window.decryptAes = async (data, rawKey, iv) => {
    const key = await crypto.subtle.importKey(
        'raw',
        rawKey,
        { name: 'AES-CBC' },
        false,
        ['decrypt']
    );

    const decrypted = await window.crypto.subtle.decrypt(
        {
            name: "AES-CBC",
            iv
        },
        key,
        data
    );

    return new Uint8Array(decrypted);
}

window.generateRsaKeys = async (keySize) => {
    const keyPair = await window.crypto.subtle.generateKey(
        {
            name: "RSA-OAEP",
            modulusLength: keySize,
            publicExponent: new Uint8Array([1, 0, 1]),
            hash: "SHA-256",
        },
        true,
        ["encrypt", "decrypt"]
    );

    const publicKey = await exportPublicKey(keyPair.publicKey);
    const privateKey = await exportPrivateKey(keyPair.privateKey);

    return { Public: publicKey, Private: privateKey };
}

async function exportPublicKey(key) {
    const exported = await window.crypto.subtle.exportKey("spki", key);
    const exportedAsString = String.fromCharCode.apply(null, new Uint8Array(exported));
    const exportedAsBase64 = window.btoa(exportedAsString);
    const pemExported = `-----BEGIN PUBLIC KEY-----\n${exportedAsBase64}\n-----END PUBLIC KEY-----`;
    return pemExported;
}

async function exportPrivateKey(key) {
    const exported = await window.crypto.subtle.exportKey("pkcs8", key);
    const exportedAsString = String.fromCharCode.apply(null, new Uint8Array(exported));
    const exportedAsBase64 = window.btoa(exportedAsString);
    const pemExported = `-----BEGIN PRIVATE KEY-----\n${exportedAsBase64}\n-----END PRIVATE KEY-----`;
    return pemExported;
}

function str2ab(str) {
    const buf = new ArrayBuffer(str.length);
    const bufView = new Uint8Array(buf);
    for (let i = 0, strLen = str.length; i < strLen; i++) {
        bufView[i] = str.charCodeAt(i);
    }
    return buf;
}

async function importKey(pem, pemHeader, pemFooter, keyFormat, rsaAlgorithmName, keyUsage) {
    // fetch the part of the PEM string between header and footer
    const pemContents = pem.substring(
        pemHeader.length,
        pem.length - pemFooter.length - 1,
    );
    // base64 decode the string to get the binary data
    const binaryDerString = window.atob(pemContents);
    // convert from a binary string to an ArrayBuffer
    const binaryDer = str2ab(binaryDerString);

    const key = await window.crypto.subtle.importKey(
        keyFormat,
        binaryDer,
        {
            name: rsaAlgorithmName,
            hash: "SHA-256",
        },
        true,
        [keyUsage],
    );

    return key;
}

async function importPublicKey(pem) {
    const publicKey = await importKey(
        pem,
        "-----BEGIN PUBLIC KEY-----",
        "-----END PUBLIC KEY-----",
        "spki",
        "RSA-OAEP",
        "encrypt"
    );
    return publicKey;
}

async function importPrivateKey(pem) {
    const privateKey = await importKey(
        pem,
        "-----BEGIN PRIVATE KEY-----",
        "-----END PRIVATE KEY-----",
        "pkcs8",
        "RSA-OAEP",
        "decrypt"
    );
    return privateKey;
}

async function importPublicKeyForVerify(pem) {
    const publicKey = await importKey(
        pem,
        "-----BEGIN PUBLIC KEY-----",
        "-----END PUBLIC KEY-----",
        "spki",
        "RSASSA-PKCS1-v1_5",
        "verify"
    );
    return publicKey;
}

async function importPrivateKeyForSign(pem) {
    const privateKey = await importKey(
        pem,
        "-----BEGIN PRIVATE KEY-----",
        "-----END PRIVATE KEY-----",
        "pkcs8",
        "RSASSA-PKCS1-v1_5",
        "sign"
    );
    return privateKey;
}

window.encryptRsa = async (data, publicPem) => {
    const publicKey = await importPublicKey(publicPem);
    const encrypted = await window.crypto.subtle.encrypt(
        {
            name: "RSA-OAEP",
        },
        publicKey,
        data
    );
    return new Uint8Array(encrypted);
}

window.decryptRsa = async (data, privatePem) => {
    const privateKey = await importPrivateKey(privatePem);
    const decrypted = await window.crypto.subtle.decrypt(
        {
            name: "RSA-OAEP",
        },
        privateKey,
        data
    );
    return new Uint8Array(decrypted);
}

window.signRsa = async (data, privatePem) => {
    const privateKey = await importPrivateKeyForSign(privatePem);
    const signature = await window.crypto.subtle.sign(
        {
            name: "RSASSA-PKCS1-v1_5"
        },
        privateKey,
        data
    );
    return new Uint8Array(signature);
}

window.verifyRsa = async (data, signature, publicPem) => {
    const publicKey = await importPublicKeyForVerify(publicPem);
    const result = await window.crypto.subtle.verify(
        "RSASSA-PKCS1-v1_5",
        publicKey,
        signature,
        data
    );
    return result;
}

window.updateMousePosition = function (canvasRef, dotNetObject, updateMouseMethodName) {
    canvasRef.addEventListener('mousemove', function (event) {
        const rect = canvasRef.getBoundingClientRect();
        const x = event.clientX - rect.left;
        const y = event.clientY - rect.top;

        // Call the Blazor method to update mouse position
        dotNetObject.invokeMethodAsync(updateMouseMethodName, x, y);
    });

    canvasRef.addEventListener('touchmove', function (event) {
        if (event.touches.length > 0) {

            const rect = canvasRef.getBoundingClientRect();
            const x = event.touches[0].clientX - rect.left;
            const y = event.touches[0].clientY - rect.top;

            // Call the Blazor method to update mouse position
            dotNetObject.invokeMethodAsync(updateMouseMethodName, x, y);
        }
    });
};

function clearLayoutStyle() {
    // Remove the MudBlazor CSS link
    const mudBlazorCss = document.querySelector('link[href*="_content/MudBlazor/MudBlazor.min.css"]');
    if (mudBlazorCss) {
        mudBlazorCss.remove();
    }

    // Remove the Blazor error UI div
    const blazorErrorUi = document.getElementById('blazor-error-ui');
    if (blazorErrorUi) {
        blazorErrorUi.remove();
    }
}

function renderHtml(containerId, htmlContent) {
    const container = document.getElementById(containerId);
    if (container) {
        container.innerHTML = htmlContent;
    }
}