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
});
