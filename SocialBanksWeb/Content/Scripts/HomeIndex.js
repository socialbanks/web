function HomeIndex() {
    var self = this;

    this.Ready = function () {
        $('#btn-create_issuance').click(self.Click_create_issuance);
    }

    this.Click_create_issuance = function () {
        $.ajax({
            url: "/home/create_issuance",
            method: "POST",
            success: self.Sucess_create_issuance
        });
    }

    this.Sucess_create_issuance = function (response) {
        console.log(response);
        bitcore = require('bitcore');
        var tx = new bitcore.Transaction(response)
        console.log(tx);
    }
}