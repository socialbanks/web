function HomeIndex() {
    var self = this;

    self.Passphrase = "";

    bitcore = require('bitcore');

    this.Ready = function () {
        $('#btn-create_issuance').click(self.Click_create_issuance);
        $('#btn-send').click(self.Click_send);
        $('#btn-enter_wallet').click(self.Click_enter_wallet);
        $('#btn-sign_transaction').click(self.Click_sign_transaction);
    }

    this.Click_sign_transaction = function () {
        var publicKeys = [self.PubKey];
        var threshold = 1;

        var utxo = new bitcore.Transaction.UnspentOutput({
            "txid": "a0a08e397203df68392ee95b3f08b0b3b3e2401410a38d46ae0874f74846f2e9",
            "vout": 0,
            "address": "mgJT8iegL4f9NCgQFeFyfvnSw1Yj4M5Woi",
            "scriptPubKey": "76a914089acaba6af8b2b4fb4bed3b747ab1e4e60b496588ac",
            "amount": 0.00070000
        });

        var tx = new bitcore.Transaction()
            .from(
                [utxo],
                publicKeys,
                threshold)
            .fee(5430); // Minimum non-dust amount

        console.log(tx);
        console.log(tx.isFullySigned());
    }

    this.Click_enter_wallet = function () {
        //https://raw.githubusercontent.com/CounterpartyXCP/counterwebdeps/master/js/util.bitcore.js
        $('#div-authenticaded-walllet').show();
        $('#div-unauthenticaded-walllet').hide();

        var Buffer = bitcore.deps.Buffer;
        var value = new Buffer($("#txt-passphrase").val());

        var hash = bitcore.crypto.Hash.sha256(value);
        var bn = bitcore.crypto.BN.fromBuffer(hash);

        self.PvtKey = new bitcore.PrivateKey(bn);
        self.PubKey = new bitcore.PublicKey(self.PvtKey);
        self.Address = self.PvtKey.toAddress();

        console.log(self.PubKey);
        console.log(self.PubKey.toBuffer());

        $('#span-public-address').text(bitcore.encoding.Base58.encode(self.PubKey.toBuffer()));
    }

    this.Click_create_issuance = function () {
        $.ajax({
            url: "/home/create_issuance",
            method: "POST",
            data:
                {
                    source: "1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK",
                    asset: "BRAZUCA",
                    quantity: 10,
                    description: ""
                },
            success: self.Sucess_create_issuance
        });
    }

    this.Click_send = function () {
        $.ajax({
            url: "/home/send",
            method: "POST",
            data:
                {
                    //source: "1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK",
                    source: "1sEAUJsjuYJ9P64Y2MxchwyDfw8hbQDNA",
                    asset: "BRAZUCA",
                    quantity: 10,
                    destination: "1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK"
                },
            success: self.Sucess_create_issuance
        });
    }

    this.Sucess_create_issuance = function (response) {
        console.log('response');
        console.log(response);
        var tx = new bitcore.Transaction(response)
        console.log(tx);

        console.log(tx.isFullySigned());

        tx.sign(self.PvtKey);
        console.log(tx);

        console.log(tx.isFullySigned());

    }
}