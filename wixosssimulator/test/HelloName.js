function ViewModel() {
    var self = this;

    self.firstName = ko.observable("山田");
    self.lastName = ko.observable("太郎");

    self.fullName = ko.computed(function () {
        return self.firstName() + " " + self.lastName();
    }, self);
};
