$(function () {
    var connection = $.hubConnection('/signalr', { useDefaultPath: false }), // 本番環境では '/wixosssimulator/signalr' なんだけど……
        crawler = connection.createHubProxy("crawler"),

        vm = {

            //self: this,
            started: ko.observable(false),

            userId: ko.observable(""),
            password: ko.observable(""),

            progressPrimary: ko.observable(0),
            progressPrimaryMessage: ko.observable(""),
            progressSecondary: ko.observable(0),
            progressSecondaryMessage: ko.observable(""),

            domain: ko.observable("Unknown"),
            domainAttribute: ko.observableArray([]),

            crawlingTable: ko.observableArray([]),
            isLoadCrawlingTable: ko.observable(false),
            loadCrawlingTable: function () {
                vm.enableToUpdate(false);
                vm.crawlingTable.removeAll();

                vm.isLoadCrawlingTable(true);
                crawler.invoke("GetCrawlingTable", vm.userId(), vm.password(), vm.domain());
            },


            isSearch: ko.observable(false),
            startSearching: function () {
                vm.isSearch(true);

                vm.isLoadCrawlingTable(true);
                var a = ko.toJSON(vm.crawlingTable());
                //crawler.invoke("GetCrawlingTable", vm.userId(), vm.password(), vm.domain());
                crawler.invoke("SearchAllDomainId", vm.domain(), ko.toJSON(vm.crawlingTable()));
                //crawler.invoke("UpdateCrawlingTable", vm.userId(), vm.password(), vm.domain(), ko.toJSON(vm.crawlingTable()));
                //vm.isSearch(false);
                //crawler.invoke("SearchAllDomainId2", vm.domain(), a);
            },

            enableToUpdate: ko.observable(false),
            startUpdating: function () {
                crawler.invoke("UpdateCrawlingTable", vm.userId(), vm.password(), vm.domain(), ko.toJSON(vm.crawlingTable()));
                //for (var i = 0; i < vm.crawlingTable().length; i++) {
                //    var sqlJsonData = ko.toJSON(vm.crawlingTable()[i]);
                //    crawler.invoke("UpdateSql", vm.userId(), vm.password(), vm.domain(), sqlJsonData);
                //}

            }

        };

    function CrawlingData(data) {
        var self = this;
        self.domainId = ko.observable(data.domainId);
        self.url = ko.observable(data.url);
        self.lastUpdated = ko.observable(data.lastUpdated);
        self.lastConfirmed = ko.observable(data.lastConfirmed);
        self.deleted = ko.observable(data.deleted);
    }

    ko.applyBindings(vm);

    crawler.on("EndSearching", function (domain) {
        vm.isSearch(false);
        vm.enableToUpdate(true);
    });

    crawler.on("SetDomainAttribute", function (domain) {
        vm.domainAttribute.push(domain);
    });
    crawler.on("SetCrawlingTable", function (crawlingTable) {
        vm.crawlingTable($.map(JSON.parse(crawlingTable), function (data, i) {
            return new CrawlingData(data);
        }));
        vm.isLoadCrawlingTable(false);
        vm.enableToUpdate(false);
        //vm.crawlingTable.push(new CrawlingData({}));
    });
    crawler.on("SetProgressPrimary", function (value, message) {
        vm.progressPrimary(value);
        vm.progressPrimaryMessage(message);
    });
    crawler.on("SetProgressSecondary", function (value, message) {
        vm.progressSecondary(value);
        vm.progressSecondaryMessage(message);
    });
    //crawler.on("SetUrl", function (url, domainId) {
    //    for (var i = 0; i < vm.crawlingTable().length; i++) {
    //        if (vm.crawlingTable()[i].domainId() == domainId) {
    //            //var a = "<a target=\"_blank\" href=\"" + url + "\">" + url + "</a>";
    //            vm.crawlingTable()[i].url(url);
    //            return;
    //        }
    //    }
    //});



    connection.start().done(function () {
        crawler.invoke("GetDomainList");
        vm.started(true);
    });
});
