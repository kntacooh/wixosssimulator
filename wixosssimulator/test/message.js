$(function () {
    var connection = $.hubConnection();
    var message = connection.createHubProxy("message");

    var viewModel = {
        started: ko.observable(false),
        form: ko.observable(),
        items: ko.observableArray(),
        send: function () {
            message.invoke("Send", viewModel.form());

            viewModel.form("");
        }
    }

    ko.applyBindings(viewModel);

    message.on("Receive", function (text) {
        viewModel.items.push(text);
    });

    connection.start(function () {
        viewModel.started(true);
    });
});
