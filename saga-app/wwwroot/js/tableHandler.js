var dataTable;

$(document).ready(function () {
    loadTable();
});


function loadTable() {
    dataTable = $('#DT_load').DataTable({
        "ajax": {
            "url": "/table/getall/",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "field1", "width": "30%" },
            { "data": "field2", "width": "30%" },
            { "data": "field3", "width": "30%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                        <a onclick="showPopup('/Table/Upsert?id='+${data}, 'Edit Item')" style='cursor:pointer;'>
                            <i class="far fa-edit"></i>
                        </a>
                        &nbsp;
                        <a  style='cursor:pointer;'
                            onclick=tableDelete('/Table/Delete?id='+${data})>
                                <i class="fas fa-trash"></i>
                        </a>
                        </div>`;
                }, "width": "10%"
            }
        ],
        "language": {
            "emptyTable": "The table is empty!"
        },
        "width": "100%"
    });
}

function showPopup(url, title) {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $('#form-modal .modal-body').html(res);
            $('#form-modal .modal-title').html(title);
            $('#form-modal').modal('show');
        }
    })
}

function tablePost(form) {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    $('#view-all').html(res.html)
                    $('#form-modal .modal-body').html('');
                    $('#form-modal .modal-title').html('');
                    $('#form-modal').modal('hide');

                    toastr.success(res.message);
                    dataTable.ajax.reload();
                }
                else {
                    $('#form-modal .modal-body').html(res.html);
                }
            },
            error: function (err) {
                console.log(err)
            }
        })
        
        return false;
    } catch (ex) {
        console.log(ex)
    }
}

function tableDelete(url) {
    if (confirm('Are you sure to delete this record ?')) {
        try {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        } catch (ex) {
            console.log(ex)
        }
    }

    return false;
}