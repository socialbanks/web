function HomeTestFabricio() {
    var self = this;

    self.Passphrase = "";

    //bitcore = require('bitcore');
    //var explorer = require('bitcore-explorers');

    this.CreateKeys = function () {
        console.log('Test');
        
        //address = 1JeMty246HPfyGJEUJqswmP8xQUiCqUjMA
        var privateKey = new bitcore.PrivateKey('b221d9dbb083a7f33428d7c2a3c3198ae925614d70210e28716cca0000000000');
        console.log(privateKey);

        var publicKey = privateKey.toPublicKey();
        console.log(publicKey);
        console.log(publicKey.toString());

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

    
    //avulso => counterparty
    this.SendBrazuca1 = function () {
        var privateKey = new bitcore.PrivateKey('b221d9dbb083a7f33428d7c2a3c3198ae925614d70210e28716cca0000000000');
        var senderPublicKey = privateKey.toPublicKey();
        var senderAddress = senderPublicKey.toAddress(bitcore.Networks.livenet);

        // After installing the Parse SDK above, intialize it with your Parse application ID and key below
        Parse.initialize("bCOd9IKjrpxCPGYQfyagabirn7pYFjYTvJqkq1x1", "mu3goJMujN0svROX9d1ssE0R7N4q1r3mC9FR8ejP");

        // Call the Cloud Code 'yodaSpeakFunction'
        Parse.Cloud.run('send', 
            {
                "source": "1JeMty246HPfyGJEUJqswmP8xQUiCqUjMA",
                "destination": "1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK",
                "quantity": 1500000000,
                "asset": "BRAZUCA",
                "pubkey": senderPublicKey.toString()
                
            },
            {
                success: function (result) {
                    console.log("#success");
                    console.log(result);

                    var rawTx = result.result;
                    console.log(rawTx);

                    var trans = new bitcore.Transaction(rawTx);

                    if (trans.outputs.count == 4) {
                        trans.outputs.splice(1, 2);
                    }

                    console.log(trans);
                    console.log("hasAllUtxoInfo");
                    console.log(trans.hasAllUtxoInfo());
                    console.log(trans.inputs[0]);
                    console.log(trans.inputs[0].outputs);

                    //trans.hasAllUtxoInfo = function () { return true; };

                    console.log("proxima linha com erro!");
                    var signedTx = trans.sign(privateKey);

                    //this.broadcast(signedTx);
                },
                error: function (error) {
                    console.log("#error");
                    console.log(error);
                }

            }
        );

    }

    //counterparty => counterparty
    this.SendBrazuca2 = function () {
        //chave privada da 1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK (counterparty)
        var privateKey = new bitcore.PrivateKey('L2BkJmqFfEuDiaGxcTmA8vrrZnvoP523SMrZKzB8seHjKPwYX8Df');
        var senderPublicKey = privateKey.toPublicKey();

        console.log(senderPublicKey);
        console.log(senderPublicKey.toString());
        var pubk2 = new bitcore.PublicKey(senderPublicKey.toString());
        console.log(pubk2);
        console.log(pubk2.toJSON());


        var senderAddress = pubk2.toAddress(bitcore.Networks.livenet);

        console.log('senderAddress');
        console.log(senderAddress);
        console.log(senderAddress.toString());

        //console.log(senderPublicKey.toBuffer());
        //console.log(bitcore.Crypto.Hash.sha256(senderPublicKey.toBuffer()));

        // After installing the Parse SDK above, intialize it with your Parse application ID and key below
        Parse.initialize("bCOd9IKjrpxCPGYQfyagabirn7pYFjYTvJqkq1x1", "mu3goJMujN0svROX9d1ssE0R7N4q1r3mC9FR8ejP");


        Parse.Cloud.run('send',
            {
                "source": "1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK",
                "destination": "1BdHqBSfUqv77XtBSeofH6XwHHczZxKRUF",
                "quantity": 1500000000,
                "asset": "BRAZUCA",
                //"pubkey": "1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK" //should be the: hex(sha256(pubkey))
                //"pubkey": senderPublicKey.toString()

            },
            {
                success: function (result) {
                    console.log("#success");
                    console.log(result);
                    if (result.error) {
                        console.log("#error");
                        console.log(result.error);
                        return;
                    }

                    var rawTx = result.result;
                    console.log(rawTx);

                    var trans = new bitcore.Transaction(rawTx);

                    /*if (trans.outputs.count == 4) {
                        trans.outputs.splice(1, 2);
                    }*/

                    console.log('trans');
                    console.log(trans);

                    console.log("hasAllUtxoInfo");
                    console.log(trans.hasAllUtxoInfo());

                    //return;
                    trans.inputs[0].addSignature(privateKey);

                    console.log('trans.inputs[0].isValidSignature()');
                    console.log(trans.inputs[0].isValidSignature());

                    var signedTx = trans.sign(privateKey);

                    //this.broadcast(signedTx);
                },
                error: function (error) {
                    console.log("#error");
                    console.log(error);
                }

            }
        );

    }

    this.SendBrazuca3 = function () {

        console.log('SendBrazuca3');
        var w = new CWHierarchicalKey('passphrase')

        console.log(w);

        var privateKey0 = w.getAddressKey(0);
        console.log(privateKey0);

        // After installing the Parse SDK above, intialize it with your Parse application ID and key below
        Parse.initialize("bCOd9IKjrpxCPGYQfyagabirn7pYFjYTvJqkq1x1", "mu3goJMujN0svROX9d1ssE0R7N4q1r3mC9FR8ejP");


        Parse.Cloud.run('send',
            {
                "source": "1Ko36AjTKYh6EzToLU737Bs2pxCsGReApK",
                "destination": "1BdHqBSfUqv77XtBSeofH6XwHHczZxKRUF",
                "quantity": 100000000,
                "asset": "BRAZUCA",
                //"pubkey": senderPublicKey.toString()

            },
            {
                success: function (result) {
                    console.log("#success");
                    console.log(result);
                    if (result.error) {
                        console.log("#error");
                        console.log(result.error);
                        return;
                    }

                    var rawTx = result.result;
                    console.log(rawTx);

                    var signedHEX = privateKey0.signRawTransaction(rawTx);

                    console.log(signedHEX);

                    var signedTx = new bitcore.Transaction(signedHEX);

                    //this.broadcast(signedTx);
                },
                error: function (error) {
                    console.log("#error");
                    console.log(error);
                }

            }
        );

    }

    this.RunAll = function () {
        //this.CreateKeys();
        //this.CreateTransaction();
        //this.SendBrazuca1();
        //this.SendBrazuca2();
        this.SendBrazuca3();
    }

    this.broadcast = function (tx) {
        insight.broadcast(tx, function (err, txid) {
            if (err != null) {
                window.console.log('Broadcast Error:', err);
            } else {
                window.console.log('txid:', txid);
            }
        });
    }

    this.InvokeCounterparty = function (method, params) {
        $.ajax({
            method: 'POST',
            url: 'http://counterparty:1234@xcp-dev.vennd.io:4000/api/',
            body:
              {
                  "jsonrpc": "2.0",
                  "id": 0,
                  "method": method,
                  "params": params
              },

            headers: {
                'Content-Type': 'application/json;charset=utf-8',
                'Accept': 'application/json, text/javascript'
            },
            success: function (httpResponse) {
                console.log("Success:");
                console.log(httpResponse.data);
            },
            error: function (httpResponse) {
                console.log("Error: " + httpResponse.text);
            }
        });
    }


    /*

var Transaction = require('bitcore').Transaction;

var t = new Transaction()
  .from({"address":"3BazTqvkvEBcWk7J4sbgRnxUw6rjYrogf9","txid":"dc2e197ab72f71912c39bc23a42d823a3aa8d469fe65eb591c086e60d14c64a0","vout":0,"ts":1418878014,"scriptPubKey":"a9146c8d8b04c6a1e664b1ec20ec932760760c97688e87","amount":0.00300299,"confirmationsFromCache":false}, ["020483ebb834d91d494a3b649cf0e8f5c9c4fcec5f194ab94341cc99bb440007f2", "0271ebaeef1c2bf0c1a4772d1391eab03e4d96a6e9b48551ab4e4b0d2983eb452b", "03a659828aabe443e2dedabb1db5a22335c5ace5b5b7126998a288d63c99516dd8"], 2)
  .to("38nw4sTs3fCH1YiBjYeQAX1t9eWMxpek8Z", 150000)
  .change("3BazTqvkvEBcWk7J4sbgRnxUw6rjYrogf9")
  .sign("L2U9m5My3cdyN5qX1PH4B7XstGDZFWwyukdX8gj8vsJ3fkrqArQo") // Also tested without this step
  .fee(13400);

console.log(t.toJSON());




    this.InvokeParse = function (method, params) {
        $.ajax({
            method: 'POST',
            url: 'https://api.parse.com/1/functions/' + method,
            body:
              {
                  "jsonrpc": "2.0",
                  "id": 0,
                  "method": method,
                  "params": params
              },

            headers: {
                'X-Parse-Application-Id': 'bCOd9IKjrpxCPGYQfyagabirn7pYFjYTvJqkq1x1',
                'X-Parse-REST-API-Key': 'NZKI1LgMGKt8qm1N91XsKeg23oJqkFP7T2X5Ntdt',

                'Content-Type': 'application/json;charset=utf-8',
                'Accept': 'application/json, text/javascript'
            },
            success: function (httpResponse) {
                console.log("#Success: " + httpResponse.result.toString());
                console.log(httpResponse.result);
                console.log(httpResponse);

                return httpResponse.result;
            },
            error: function (httpResponse) {
                console.log("#Error: " + httpResponse.text);
                console.log(httpResponse);
            }
        });
    }
    */


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