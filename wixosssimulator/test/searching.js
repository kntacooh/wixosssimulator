$(function () {
    var connection = $.hubConnection('/wixosssimulator/signalr', { useDefaultPath: false });
    var searching = connection.createHubProxy("searching");
    var viewModel = {

        started: ko.observable(false),

        domain: ko.observable("Unknown"),
        domainList: ko.observableArray(),

        isSearch: ko.observable(false),
        startSearch: function () {
            viewModel.urls.removeAll();
            viewModel.isSearch(true);
            searching.invoke("GetUrls", viewModel.domain());
            viewModel.isSearch(false);
        },
        urls: ko.observableArray(),



        hello: ko.observable(),
        counter: ko.observable(0),
        form: ko.observable(),
        items: ko.observableArray(),
        helloworld: function () {
            searching.invoke("HelloWorld", viewModel.form());
            viewModel.form("");
        },
        hello: function () {
            searching.invoke("Hello");
        },

    };

    ko.applyBindings(viewModel);

    searching.on("Hello", function (text) {
        if (text) {
            viewModel.items.push(text);
        } else {
            viewModel.counter(viewModel.counter() + 1);
            viewModel.items.push("hello, " + viewModel.counter() + "回目");
        }
    });
    searching.on("SetDomainName", function (domain) {
        viewModel.domainList.push(domain);
    });

    searching.on("SetUrl", function (url) {
        viewModel.urls.push(url);
    });

    connection.start().done(function () {
        searching.invoke("GetDomainList");
        viewModel.started(true);
    });
});



