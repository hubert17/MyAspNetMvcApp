if (window.top !== window.self) {
    $('.navbar').hide();
    $('.container').hide();

    var loc = window.top.location.href, index = loc.indexOf('#');
    if (index > 0)
        window.top.location.replace('/Account/Login?ReturnUrl=' + loc.substring(0, index));
    else
        window.top.location.replace('/Account/Login');
}

$(document).ready(function () {
    $("#btn-forgot").click(function () {
        var email = $('#fpemail').val();
        if (!email) {
            return;
        }
        $(this).attr('disabled', true);
        $(this).html('Requesting...');
        var value;
        $.ajax({
            type: "POST",
            url: '/Account/ForgotPassword?email=' + email,
            success: function (res) {
                $('#forgotform').hide();
                if (res == "1") {
                    $('#forgotalert').html('Reset password request for ' + email + ' has been sent. Please check your inbox.');
                }
                else {
                    $('#forgotalert').html('Check your inbox.');
                }
            }
        });
    });
});
