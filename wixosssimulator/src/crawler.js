$(function () {
    var connection = $.hubConnection('/signalr', { useDefaultPath: false }),
        crawler = connection.createHubProxy("crawler"),
        crawlerViewModel = {

            started: ko.observable(false),

            userId: ko.observable(""),
            password: ko.observable(""),

            progressPrimary: ko.observable(0),
            progressPrimaryMessage: ko.observable(""),
            progressSecondary: ko.observable(0),
            progressSecondaryMessage: ko.observable(""),

            domainId: ko.computed({
                read: function () {
                    return this._domainId;
                },
                write: function (value) {
                    this._domainId = value;
                    crawler.invoke("GetUrlFromDomainId", crawlerViewModel.domain(), this._domainId)
                }
            }),
            url: ko.observable(""),
            
            domain: ko.computed({
                read: function () {
                    return this._domain;
                },
                write: function (value) {
                    this._domain = value;
                    crawler.invoke("GetSqlData", crawlerViewModel.userId(), crawlerViewModel.password(), crawlerViewModel.domain())
                }
            }),
            domainAttribute: ko.observableArray([]),
            isSearch: ko.observable(false),
            startSearch: function () {
                crawlerViewModel.idList.removeAll();
                crawlerViewModel.isSearch(true);
                crawler.invoke("SearchAllDomainId", crawlerViewModel.domain());
                crawlerViewModel.isSearch(false);
            },
            idList: ko.observableArray([])

        };
        //crawlingTable = {
        //    domainId: ko.computed({
        //        read: function () {
        //            return this._domainId;
        //        },
        //        write: function (value) {
        //            this._domainId = value;
        //            crawler.invoke("GetUrlFromDomainId", crawlerViewModel.domain(), this._domainId)
        //        }
        //    }),
        //    url: ko.observable(""),
        //    updated: ko.observable("")
        //};



    ko.applyBindings(crawlerViewModel);

    crawler.on("SetDomainAttribute", function (domain) {
        crawlerViewModel.domainAttribute.push(domain);
    });
    crawler.on("SetUrl", function (url) {
        crawlerViewModel.url(url);
    });
    crawler.on("SetProgressPrimary", function (value, message) {
        crawlerViewModel.progressPrimary(value);
        crawlerViewModel.progressPrimaryMessage(message);
    });
    crawler.on("SetProgressSecondary", function (value, message) {
        crawlerViewModel.progressSecondary(value);
        crawlerViewModel.progressSecondaryMessage(message);
    });



    connection.start().done(function () {
        crawler.invoke("GetDomainList");
        crawlerViewModel.started(true);
    });
});
