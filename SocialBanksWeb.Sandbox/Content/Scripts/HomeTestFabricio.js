function HomeTestFabricio() {
    var self = this;

    self.Passphrase = "";

    bitcore = require('bitcore');

    this.CreateKeys = function () {
        console.log('Test');

        //address = 1JeMty246HPfyGJEUJqswmP8xQUiCqUjMA
        var privateKey = new bitcore.PrivateKey('b221d9dbb083a7f33428d7c2a3c3198ae925614d70210e28716cca0000000000');

        var publicKey = privateKey.toPublicKey();
        var address = publicKey.toAddress(bitcore.Networks.livenet);

        if (bitcore.Address.isValid(address)) {
            console.log('is valid!');
        }

        console.log(address);
        console.log(address.toString());
    }

    this.CreateTransaction = function () {

        var receiverAddress = "1FTuKcjGUrMWatFyt8i1RbmRzkY2V9TDMG"; //Fabricio Bread Wallet (iPhone)

        //TODO: how can I get the unspent transactions?
        var txId = "025c3db2533f0405193607baf0b02395813d0fa31fd129c29d8d61f03c7a6829"; //Found using https://blockchain.info/address/1JeMty246HPfyGJEUJqswmP8xQUiCqUjMA
        var outputIndex = 0;
        var address = "1JeMty246HPfyGJEUJqswmP8xQUiCqUjMA"; //Wallet.bitcoinAddress (test address)
        var script = bitcore.Script.buildPublicKeyHashOut(bitcore.Address.fromString(receiverAddress)); //P2PKH 
        var value = 15000;

        var utxo = {
            "txId": txId,
            "outputIndex": outputIndex,
            "address": address,
            "script": script.toHex(),
            "satoshis": 15000
        };

        console.log("utxo:");
        console.log(utxo);
        console.log(script.toHex());

        //var utxo = {
        //    "txId": "115e8f72f39fad874cfab0deed11a80f24f967a84079fb56ddf53ea02e308986",
        //    "outputIndex": 0,
        //    "address": "17XBj6iFEsf8kzDMGQk5ghZipxX49VXuaV",
        //               76a914 9ea84056a5a9e294d93f11300be51d51868da693 88ac
        //    "script": "76a914 47862fe165e6121af80d5dde1ecb478ed170565b 88ac",
        //    "satoshis": 50000
        //};


        var privateKey = new bitcore.PrivateKey('b221d9dbb083a7f33428d7c2a3c3198ae925614d70210e28716cca0000000000');
        //var publicKey = new bitcore.PublicKey(privateKey);

        var transaction = new bitcore.Transaction()
          .from(utxo)
          .to(receiverAddress, value)
          .change(address)
          .sign(privateKey)
          .fee(0);

        console.log("transaction.isFullySigned():");
        console.log(transaction.isFullySigned());
        console.log(transaction);

        bitcore.Transaction.FEE_PER_KB = 0;
        console.log(transaction.serialize());
    }

    this.SendBrazuca = function () {
        //$.ajax({
        //    url: "/home/send",
        //    method: "POST",
        //    data:
        //        {
        //            //source: "1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK",
        //            source: "1sEAUJsjuYJ9P64Y2MxchwyDfw8hbQDNA",
        //            asset: "BRAZUCA",
        //            quantity: 10,
        //            destination: "1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK"
        //        },
        //    success: self.Sucess_create_issuance
        //});
    }

    this.RunAll = function () {
        this.CreateKeys();
        this.CreateTransaction();
        this.SendBrazuca();
    }

    /*
    

        bitcore = require('bitcore');
        var buffer = require('buffer');

        document.write("<pre>");

        var passphrase = 'correct horse battery staple';
        //var buf = new Buffer(passphrase);
        var hash = bitcore.crypto.Hash.sha256(encoding.GetBytes(passphrase));
        var bn = bitcore.crypto.BN.fromBuffer(hash);

        var address = new bitcore.PrivateKey(bn).toAddress();

        document.write("Address: " + address + "<br>");

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
*/
}