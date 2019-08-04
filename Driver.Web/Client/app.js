const myTextArea = document.getElementById("code");
const myCodeMirror = CodeMirror.fromTextArea(myTextArea, {
    mode: "text/x-csharp",
    theme: "pastel-on-dark",
    lineNumbers: true,
    indentUnit: 4,
    extraKeys: { "Ctrl-Space": "autocomplete" },
});

const myForm = document.getElementById("aiForm");
myForm.onsubmit = (event) => {
    event.preventDefault();

    const driver = getDriver();
    sendDriver(driver);

    return false;
};

const getDriver = () => {
    return {
        code: document.getElementById("code").value,
        trackState: {
            car: {
                speed: document.getElementById("speed").value,
                position: document.getElementById("position").value
            }
        }
    };
};

const sendDriver = (driver) => {
    const request = new XMLHttpRequest();
    request.open('POST', '/api/Driver', true);
    request.setRequestHeader('Content-Type', 'application/json; charset=UTF-8');
    request.onload = function () {
        const resp = JSON.parse(this.response);
        if (this.status >= 200 && this.status < 400 && resp.isOk) {
            drawResponse(resp);
            console.log(resp);
        } else {
            drawError(resp);
            console.error(resp);
        }
    };

    request.onerror = function () {
        console.error("Connection failed");
    };

    request.send(JSON.stringify(driver));
};

const drawResponse = (response) => {
    document.getElementById("setSpeed").innerText = response.speed;
    document.getElementById("executionTime").innerText = response.executionTime;

    const variables = document.getElementById("variables");
    variables.innerText = null;
    response.variables.forEach((variable) => {
        if (variable.name != "output") {
            variables.appendChild(newVariable(variable));
        }
    });

    document.getElementById("errors").innerText = null;
};

const newVariable = (variable) => {
    var row = document.createElement("tr");
    row.appendChild(newTd(variable.name));
    row.appendChild(newTd(variable.value));
    row.appendChild(newTd(variable.type));

    return row;
};

const drawError = (response) => {
    document.getElementById("setSpeed").innerText = "N/A";
    document.getElementById("executionTime").innerText = "N/A";

    document.getElementById("variables").innerText = null;

    var errors = document.getElementById("errors");
    errors.innerText = null;
    response.errors.forEach((error) => {
        errors.appendChild(newError(error));
    });
};

const newError = (error) => {
    var row = document.createElement("tr");
    row.appendChild(newTd(error.severity));
    row.appendChild(newTd(error.message));
    row.appendChild(newTd(error.startLinePosition + 1));
    row.appendChild(newTd(error.endLinePosition + 1));

    return row;
};

const newTd = (str) => {
    var td = document.createElement("td");
    var content = document.createTextNode(str);
    td.appendChild(content);

    return td;
};

sendDriver(getDriver());