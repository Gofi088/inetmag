$(document).ready(function () {
    $("#btnSubscribe").click(function () {
        $.ajax({
            url: "/Home/Subscribe",
            type: "POST",
            data: { email: $("#inputSubscribe").val() },
            dataType: "html",
            success: function (data) { AlertFooterMessage(data); }
        });
    });
    
    $("#liHome").addClass("current");

    SetNavigation();

    $("#btnFilter").click(function () {
        if ($("#formFilter").css("display") === "none") {
            $("#btnFilter").html("Показать окно фильтрации");
            $("#formFilter").show();
        }
        else {
            $("#btnFilter").html("Скрыть окно фильтрации");
            $("#formFilter").hide();
        }
    });

    $("#btnApply").click(function (evt) {
        evt.preventDefault();

        var jsonArr = "[";

        var other_data = $("#ApplyForm").serializeArray();
        $.each(other_data, function (_key, input) {
            jsonArr += "{'Key':'" + input.name + "','Value':'" + input.value + "'},";
        });

        jsonArr = jsonArr.slice(0, -1);
        jsonArr += "]";

        $.ajax({
            url: "/Home/SetSearchValue",
            type: "POST",
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(jsonArr),
            async: false,
            success: function () {
                location.reload();
            },
            error: function () { console.log("Ошибка"); }
        });
    });
});

function HideGoods(data) {
    $(data.divClass).eq(0).css("opacity", "0.2");
    $(data.divClass).eq(0).attr("disabled", "disabled");
    $("#summDiv").html(" Итоговая сумма: " + data.summ);
}

function AjaxLoad(data) {
    //var urlReturn = JSON.parse(data).url;

    var urlReturn = "";

    if (data.message !== undefined) {
        urlReturn = data.url + "?Message=" + data.message;
        location.href = urlReturn;        
    }
    else {
        document.location.reload(true);
    }
}

function AlertMessage(data) {

    var divId = "#" + data.divId;
    var btnId = "#" + data.btnId;

    $(divId).css("display", "block");
    $(divId).html("<p>" + data.message + "</p>");

    if (btnId !== "#undefined") { $(btnId).attr("disabled", true); }

    setTimeout(function () {
        $(divId).css("display", "none");
        if (btnId !== "#undefined") { $(btnId).removeAttr("disabled"); }
        if (data.home === true) { location.href = "/Admin/Index"; }
        if (data.userHome === true) { location.href = "/Home/Index"; }
    }, data.time);
}


function IndexBeforeSend() {
    $("#PartialViewId").html("<img src=\"../gif/admin/Loading.gif\"/>");
}

function IndexLoadData(data) {
    $("#PartialViewId").html(data);
}

function BeforeSendSignIn() {
    BeforeSend("#signInBlock");
}

function BeforeSend(blockId) {
    $(blockId).css("text-align", "center");
    $(blockId).html("<img src=\"../gif/admin/Loading.gif\"/>");
}

function AfterSendError(blockId) {
    $(blockId).css("text-align", "center");
    $(blockId).html("<img src=\"../img/admin/adminError.png\" style=\"width: 200px; height: 200px; margin-top: 10%;\"/><br>");
}

function AlertFooterMessage(data) {
    $("#infoBlock").html(JSON.parse(data).message);
    $("#btnSubscribe").attr("disabled", true);

    setTimeout(function () {
        $("#btnSubscribe").removeAttr("disabled");
        $("#infoBlock").html("");
    }, 3000);
}

function SetNavigation() {
    var path = window.location.pathname;
    path = path.replace(/\/$/, "");
    path = decodeURIComponent(path);

    $(".header__nav a").each(function () {
        var href = $(this).attr('href');
        try {
            if (path.substring(0, href.length) === href) {
                if (path === "/Home/Posts") {
                    $("#liHome").removeClass("current");
                    $("#liPosts").addClass("current");
                }
                else {
                    if (path !== "" || path !== "/") {
                        $("#liHome").removeClass("current");
                    }

                    $(this).closest("li").addClass("current");
                }
            }
        } catch (e) { /*console.log(e.message);*/ }
    });
}

function ShowPrettyWindow(url) {
    var lang = $("#LangValue").val();

    var title = $("#idPrettyWindowTittle").val();
    var content = $("#idPrettyWindowContent").val();
    var confirmButtonText = $("#idPrettyWindowConfirmBtn").val();
    var cancelButtonText = $("#idPrettyWindowCancelBtn").val();
    var errorMessage = $("#idPrettyWindowErrorMsg").val();

    $.sweetModal({
        theme: $.sweetModal.THEME_DARK,
        title: title,
        content: content,
        type: $.sweetModal.TYPE_MODAL,
        buttons: {
            cancelAction: {
                label: cancelButtonText,
                classes: "bordered"
            },
            deletionAction: {
                label: confirmButtonText,
                classes: "bordered",
                action: function () {
                    $.ajax({
                        url: url,
                        type: "POST",
                        dataType: "html",
                        success: function (data) {
                            var urlReturn = JSON.parse(data).url;
                            location.href = urlReturn;
                        },
                        error: function () {
                            var someObject = { url: "/Home/Error?Message=" + errorMessage };
                            OpenReturnUrl(someObject);
                        }
                    });
                }
            }
        }
    });
}

function ReloadPage() { document.location.reload(true); }

function OpenReturnUrl(data) { location.href = data.url; }