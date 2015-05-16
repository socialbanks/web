function CreateSocialBank($) {
    var self = this;

    this.Ready = function () {
        $('#btn-create-bank').click(self.Click_create_bank);
        $('#btn-create-user').click(self.Click_create_user);
        $('#btn-create-passphrase').click(self.Click_create_passphrase);
        $('#btn-finish').click(self.Click_finish);
    }

    this.Click_create_bank = function () {
        $('#div-create-bank').hide();
        $('#div-create-user').show();
    }

    this.Click_create_user = function () {
        $('#div-create-user').hide();
        $('#div-create-passphrase').show();        
    }

    this.Click_create_passphrase = function () {
        $('#passphrase').val('yai yai yai yai ');
    }

    this.Click_finish = function () {
        self.Save();
    }

    this.Save = function () {

        var data = $('#form-create-bank').serialize();

        $.ajax(
            {
                url: 'PostSocialBank',
                method: 'POST',
                data: data,
                success: self.success_PostSocialBank
            }
            );
    }

    this.success_PostSocialBank = function (response) {

        var data = $('#form-create-user').serialize();

        $.ajax(
            {
                url: 'PostUser',
                method: 'POST',
                data: data,
                success: self.success_PostUser
            }
            );

    }

    this.success_PostUser = function (response) {
        console.log(response);
    }

}