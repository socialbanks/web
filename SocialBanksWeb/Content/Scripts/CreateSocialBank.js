function CreateSocialBank($) {
    var self = this;

    this.Ready = function () {
        $('#btn-create').click(self.Click_create);
    }

    this.Click_create = function () {
        var data = $('#form-create').serialize();
        
        $.ajax(
            {
                url: 'PostCreateSocialBank',
                method:'POST',
                data: data,
                success: self.success_createSocialBank
            }
            );
    }

    this.success_createSocialBank = function (response) {
        console.log(response);
    }

}