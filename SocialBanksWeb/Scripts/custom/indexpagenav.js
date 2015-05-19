$(document).ready(function () {

    $('.social-banks-list-link').click(function () {
        $('body').scrollTo($('#sb-section'), 800, { easing: 'swing' });
    });
    $('.digital-currencies-link').click(function () {
        $('body').scrollTo($('#dc-section'), 1600, { easing: 'swing' });
    });
    $('.our-solution-link').click(function () {
        $('body').scrollTo($('#os-section'), 2400, { easing: 'swing' });
    });
    $('.who-we-are-link').click(function () {
        $('body').scrollTo($('#who-section'), 2400, { easing: 'swing' });
    });

    $('div.projects').find('div.sbb-balance').each(function () {
        var linkT = $(this);
        var a = linkT.attr('address');
        $.ajax({
            url: 'https://blockchain.info/q/addressbalance/' + a,
            type: 'get',
        }).done(function (msg) {
            linkT.find('span.sbb-balance-value').text('$'+ (msg/100000000));
            linkT.removeAttr('hidden');
        });
    });
});
