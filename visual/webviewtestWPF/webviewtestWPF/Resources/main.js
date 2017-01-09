//Fix for IE7 and lower 
if (!document.querySelectorAll) {
    document.querySelectorAll = function (selectors) {
        var style = document.createElement('style'), elements = [], element;
        document.documentElement.firstChild.appendChild(style);
        document._qsa = [];

        style.styleSheet.cssText = selectors + '{x-qsa:expression(document._qsa && document._qsa.push(this))}';
        window.scrollBy(0, 0);
        style.parentNode.removeChild(style);

        while (document._qsa.length) {
            element = document._qsa.shift();
            element.style.removeAttribute('x-qsa');
            elements.push(element);
        }
        document._qsa = null;
        return elements;
    };
}

//Fix for IE7 and lower 
if (!document.querySelector) {
    document.querySelector = function (selectors) {
        var elements = document.querySelectorAll(selectors);
        return (elements.length) ? elements[0] : null;
    };
}

function dockerClientRequest(command) {
    try {
        window.external.docker(command);
    } catch (err) {
        alert('error');
    }
}

function redHeader() {
    document.querySelector('h1').innerHTML = 'Docker is not running';
}

function docker_info(infoJSON) {
    var info = JSON.parse(infoJSON);
    document.querySelector('h1').innerHTML = 'version: ' + info.ServerVersion;
}

dockerClientRequest('info');
