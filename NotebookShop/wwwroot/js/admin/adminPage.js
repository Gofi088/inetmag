$(document).ready(function () {
    $("#idHrHtmlEditor").hide();
    $("#idLoadEditor").hide();

    $('#idPageSize').mouseup(function () {
        var open = $(this).data("isopen");
        if (open) {
            $.ajax({
                url: "/Admin/SetPageSize",
                type: "GET",
                data: { PageSize: $('select[id=idPageSize] option:selected').val() },
                dataType: "html",
                success: function (data) {
                    $("#idCountRecords").text(data);
                    $("#idCountRecords").css("color", "black");

                    var someObject = {
                        method: "Load",
                        table: $('select[id=idTables] option:selected').val(),
                        url: "/Admin/ListView"
                    };
                    AjaxLoad(someObject);
                },
                error: function () {
                    $("#idCountRecords").text(data);
                    $("#idCountRecords").css("color", "red");
                }
            });
        }
        $(this).data("isopen", !open);
    });

    $('#LoadId').click(function () {
        if ($('select[id=idTables] option:selected').index() === 0) { return; }

        var someObject = {
            method: "Load",
            table: $('select[id=idTables] option:selected').val(),
            url: "/Admin/ListView"
        };
        AjaxLoad(someObject);
    });

    $('#AddId').click(function () {
        if ($('select[id=idTables] option:selected').index() === 0) { return; }

        var someObject = {
            method: "Add",
            table: $('select[id=idTables] option:selected').val(),
            url: "/Admin/AddEditShow"
        };
        AjaxLoad(someObject);
    });

    $('#EditId').click(function () {
        if ($('select[id=idTables] option:selected').index() === 0) { return; }

        var someObject = {
            method: "Update",
            table: $('select[id=idTables] option:selected').val(),
            url: "/Admin/AddEditShow"
        };
        AjaxLoad(someObject);
    });

    $('#SearchField').on('input', function () {

        if ($('select[id=idTables] option:selected').index() === 0) { return; }

        $.ajax({
            url: "/Admin/SetSearchValue",
            type: "GET",
            data: { SearchValue: $('#SearchField').val() },
            dataType: "html",
            success: function () {
                var someObject = {
                    method: "Load",
                    table: $('select[id=idTables] option:selected').val(),
                    url: "/Admin/ListView"
                };
                AjaxLoad(someObject);
            },
            error: function () {
                $("#tdPartialViewId").css("text-align", "center");
                $("#PartialViewId").html("<img src=\"../images/admin/adminError.png\" style=\"width: 200px; height: 200px; margin-top: 10%;\"/><br>");
            }
        });
    });
    
    $("#idTables").change(function () {
        if (this.value === "posts" || this.value === "books" || this.value === "scientists" || this.value === "aboutproject") {
            $("#idHrHtmlEditor").show();
            $("#idLoadEditor").show();
        }
        else {
            $("#idHrHtmlEditor").hide();
            $("#idLoadEditor").hide();
        }
    });    
});

function ModalHide() {
    $(".modal").modal("hide");
    $("body").removeClass("modal-open");
    $('.modal-backdrop').remove();
}

function UploadImg() {
    var currentTable = $('select[id=idTables] option:selected').val();

    if (currentTable === "posts" || currentTable === "books" || currentTable === "scientists") {
        var status = $("select[id=fileInputStatus] option:selected").val();
        var table = $("#currentTable").val();
        var editId = $('input[name=Id]').val();

        var formData = new FormData();
        formData.append("fileImg", $("#IdFileImg")[0].files[0]);
        formData.append("status", status);
        formData.append("table", table);
        formData.append("id", editId);

        var other_data = $("#ApplyForm").serializeArray();
        $.each(other_data, function (_key, input) {
            formData.append(input.name, input.value);
        });

        $.ajax({
            url: "/Admin/ApplyImgFile",
            type: "POST",
            data: formData,
            processData: false,  // tell jQuery not to process the data
            contentType: false,  // tell jQuery not to set contentType
            success: function () {
                console.log("success upload img");
            },
            error: function () {
                console.log("UploadImg() method error");
            }
        });
    }
}

function AjaxLoad(data) {
    if ($('select[id=idTables] option:selected').index() === 0) { return; }

    $.ajax({
        url: data.url,
        type: "POST",
        data: {
            Table: $('select[id=idTables] option:selected').val(),
            Page: data.page,
            Method: data.method,
            Message: data.message
        },
        dataType: "html",
        beforeSend: function () {
            $("#tdPartialViewId").css("text-align", "center");
            $('#PartialViewId').html("<img src=\"../gif/admin/Loading.gif\"/>");
        },
        success: function (data) {
            $("#tdPartialViewId").css("text-align", "left");
            $("#PartialViewId").html(data);
        },
        error: function () {
            $("#tdPartialViewId").css("text-align", "center");
            $("#PartialViewId").html("<img src=\"../images/admin/adminError.png\" style=\"width: 200px; height: 200px; margin-top: 10%;\"/><br>");
        }
    });

    $("#TableId").html("<p style=\"margin-left: 5px;\">" + $('select[id=idTables] option:selected').text() + "</p>");
}

function ReloadPage() { document.location.reload(true); }

function OpenReturnUrl(data) { location.href = data.url; }