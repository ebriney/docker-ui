function dockerClientRequest(command) {
    try {
        webkit.messageHandlers.docker.postMessage(command);
    } catch(err) {
        console.log('error');
    }
}

function redHeader() {
    document.querySelector('h1').innerHTML = 'Docker is not running';
}

function docker_info(info) {
    document.querySelector('h1').innerHTML = 'version: ' + info.ServerVersion;
}

function docker_version(info) {
    document.querySelector('h1').innerHTML = 'version: ' + info.Version;
}

dockerClientRequest('version');
