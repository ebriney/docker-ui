function dockerClientRequest(command) {
    try {
        window.external.docker.postMessage(command);
    } catch (err) {
        console.log('error');
    }
}

function redHeader() {
    document.querySelector('h1').innerHTML = 'Docker is not running';
}

function docker_info(info) {
    document.querySelector('h1').innerHTML = 'version: ' + info.ServerVersion;
}

dockerClientRequest('info');
