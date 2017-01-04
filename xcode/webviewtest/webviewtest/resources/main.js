function getDockerInfo() {
    try {
        webkit.messageHandlers.info.postMessage("");
    } catch(err) {
        console.log('error');
    }
}

function redHeader() {
    document.querySelector('h1').innerHTML = "Docker is not running";
}

function dockerInfoChanged(dockerInfo) {
    document.querySelector('h1').innerHTML = "version: " + dockerInfo.ServerVersion;
}

getDockerInfo();
