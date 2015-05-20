function CreateSocialBank($) {
    var self = this;

    this.Ready = function () {
        $('#btn-create-bank').click(self.Click_create_bank);
        $('#btn-finish').click(self.Click_finish);
    }

    this.Click_create_bank = function () {
        if (!$('#form-create-bank').valid()) {
            return;
        }
        $('#div-create-bank').hide();
        $('#div-create-user').show();
    }

    this.Click_finish = function () {
        $('#error-message').html('');

        if (!$('#form-create-user').valid()) {
            return;
        }
        self.Save();
    }

    this.Save = function () {

        var data = $('#form-create-bank').serialize();

        $.ajax(
            {
                url: 'PostSocialBank',
                method: 'POST',
                data: data,
                success: self.success_PostSocialBank,
                error: self.error
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
                success: self.success_PostUser,
                error: self.error
            }
            );

    }

    this.success_PostUser = function (response) {
        console.log(response);
    }

    this.error = function (response) {
        console.log(response);
        $('#error-message').html(response.responseText);
    }

}