$(document).ready(function () {
    $("form").validate({
        debug: true,
        errorPlacement: function (label, element) {
            $(element).nextAll().remove().end().after('<span class="input-notification error" style=" display: inline-block;">' + $(label).html() + '</span>')
        },
        success: function (label) {
            $("#" + $(label).attr("for")).nextAll().remove().end().after('<span class="input-notification success png_bg">通过验证</span>');
        },
        submitHandler: function (form) {
            if (subbefore()) {
                $(form).ajaxSubmit({
                    type: "POST",
                    success: function (result) {
                        if (typeof (result) != "object")
                            result = $.parseJSON(result.replace(/<[^>]+>/g, ""));
                        alert(result.msg);
                        if (result.status == 1)
                            subafter(result);
                    }
                })
            }
        }
    })
});
function subbefore() {
    return true;
}
function subafter(result) {
}