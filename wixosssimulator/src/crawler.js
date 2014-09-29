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
                crawler.invoke("SearchAllDomainId", vm.domain(), ko.toJSON(vm.crawlingTable()));
                //crawler.invoke("SearchAllDomainId2", vm.domain());
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

    function CrawlingData() {
        var self = this;
        self.domainId = ko.observable("");
        self.url = ko.observable("");
        self.content = ko.observable("");
        self.lastUpdated = ko.observable("");
        self.lastConfirmed = ko.observable("");
        self.deleted = ko.observable("");
        //self.isExistInSql = ko.observable(false);
        //self.isCrawled = ko.observable(false);

        //self.domain = ko.observable("");
        //self.domainId = ko.computed({
        //    read: function () {
        //        return this._domainId;
        //    },
        //    write: function (value) {
        //        this._domainId = value;
        //        crawler.invoke("GetUrlFromDomainId", self.domain(), this._domainId)
        //    }
        //});
    }

    function CrawlingData2(data) {
        var self = this;
        self.domainId = ko.observable(data.domainId);
        self.url = ko.observable(data.url);
        self.content = ko.observable(data.content);
        self.lastUpdated = ko.observable(data.lastUpdated);
        self.lastConfirmed = ko.observable(data.lastConfirmed);
        self.deleted = ko.observable(data.deleted);
    }

    ko.applyBindings(vm);

    crawler.on("AddCrawledData", function (domainId, url) {
        for (var i = 0; i < vm.crawlingTable().length; i++) {
            if (vm.crawlingTable()[i].domainId() == domainId) {
                //vm.crawlingTable()[i].isCrawled(true);
                return;
            }
        }

        var crawlingData = new CrawlingData();
        crawlingData.domainId(domainId);
        crawlingData.url(url);
        //crawlingData.isCrawled(true);
        vm.crawlingTable.push(crawlingData);
    });

    crawler.on("EndSearching", function (domain) {
        vm.isSearch(false);
        vm.enableToUpdate(true);
    });

    crawler.on("SetDomainAttribute", function (domain) {
        vm.domainAttribute.push(domain);
    });
    //crawler.on("SetSqlData", function (domainId, url, lastUpdated, lastConfirmed) {
    //    var crawlingData = new CrawlingData();
    //    crawlingData.domainId(domainId);
    //    crawlingData.url(url);
    //    crawlingData.lastUpdated(lastUpdated);
    //    crawlingData.lastConfirmed(lastConfirmed);
    //    //crawlingData.isExistInSql(true);
    //    vm.crawlingTable.push(crawlingData);
    //});
    crawler.on("SetCrawlingTable", function (crawlingTable) {
        vm.crawlingTable($.map(JSON.parse(crawlingTable), function (data, i) {
            return new CrawlingData2(data);
        }));
        vm.isLoadCrawlingTable(false);
        //vm.crawlingTable.push(new CrawlingData2({}));
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
