$(document).ready(function () {

});

// Retrieve Examples menu items
$(document).ready(function () {
    $.ajax({
        url: "/app/examples/",
        success: function (result) {
            $("#myaspnetmvcapp-menu").append(result);
        }
    });
});