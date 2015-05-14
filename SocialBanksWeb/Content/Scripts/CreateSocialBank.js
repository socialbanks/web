function CreateSocialBank($) {
    var self = this;

    this.Ready = function () {
        $('#btn-create').click(self.Click_create);
    }

    this.Click_create = function () {
        $.ajax(
            {
                url: 'createSocialBank',
                method:'POST',
                data:
                    {
                        name: "a",
                        socialMoneyName:"AAAA",
                    },
                success: self.success_createSocialBank
            }
            );
    }

    this.success_createSocialBank = function (response) {
        console.log(response);
    }

}