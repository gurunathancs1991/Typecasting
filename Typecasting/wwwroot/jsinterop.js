window.comp = {
    init: function (dotnet) {
        window.compInst = dotnet;
        var object = document.getElementById('gencomp');
        object.addEventListener("change", window.comp.change);      
    },
    change: function (e) {
        var data = JSON.stringify({ value: document.getElementById('gencomp').value });
        var promise = window.compInst.invokeMethodAsync('Trigger', "change", data);
        promise.then(function (data) {
            var parsedData = JSON.parse(data);
            document.getElementById('gencomp').value = parsedData.value;
        });
    }
};
