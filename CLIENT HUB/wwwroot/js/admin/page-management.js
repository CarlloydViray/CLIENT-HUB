var clientList = {};
var menuList = {};
var appList = {};
var roleList = {};


$(document).ready(function () {
    $('#filterClient').on('input', function () {
        var filterValue = $(this).val().toLowerCase();
        $('#clientList li').each(function () {
            var listItemText = $(this).text().toLowerCase();
            $(this).toggle(listItemText.includes(filterValue));
        });
    });

    $('#filterMenus').on('input', function () {
        var filterValue = $(this).val().toLowerCase();
        $('#menuList li').each(function () {
            var listItemText = $(this).text().toLowerCase();
            $(this).toggle(listItemText.includes(filterValue));
        });
    });

    $('#filterApps').on('input', function () {
        var filterValue = $(this).val().toLowerCase();
        $('#appList li').each(function () {
            var listItemText = $(this).text().toLowerCase();
            $(this).toggle(listItemText.includes(filterValue));
        });
    });

    loadData();

});

function loadData() {
    return new Promise(resolve => {
        get_all_menus();
        get_all_apps();
        get_all_clients();
        setTimeout(function () {
            resolve();
        }, 1000);
    });
}

// Usage
loadData().then(function () {
    get_pages();
});


function get_pages() {
    fetch('/api/admin/role-management/get-all-roles')
        .then(response => {
            // Check if the response status is OK (HTTP 200)
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }

            // Parse the response body as JSON
            return response.json();
        })
        .then(data => {
            const tableHead = document.querySelector("#dataTable thead");
            const tableBody = document.getElementById("table-body");
            const headerRow = document.createElement("tr");

            const jsonObject = JSON.parse(data);
            roleList = [];


            jsonObject.forEach((element, index) => {

                const headers = Object.keys(element);

                if (index == 0) {
                    headers.forEach(header => {
                        if (header != 'is_admin') {
                            var headerCell = document.createElement("th");
                            headerCell.textContent = header;
                            headerRow.appendChild(headerCell);
                        }
                    });

                    var headerCell = document.createElement("th");
                    headerCell.textContent = "modify";
                    headerRow.appendChild(headerCell);

                    tableHead.appendChild(headerRow);
                }

                var row = tableBody.insertRow();
                var itemIndex = 0;


                roleList[element["id"]] = element["name"];

                loopRoleList(headers, element, row);

            });



        })
        .catch(error => {
            // Handle any errors that occurred during the fetch
            console.error('Fetch error:', error);
        });

}

async function refresh_roles() {

    await new Promise(resolve => setTimeout(resolve, 1000));

    fetch('/api/admin/role-management/get-all-roles')
        .then(response => {
            // Check if the response status is OK (HTTP 200)
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }

            // Parse the response body as JSON
            return response.json();
        })
        .then(data => {
            const tableHead = document.querySelector("#dataTable thead");
            const tableBody = document.getElementById("table-body");

            const jsonObject = JSON.parse(data);
            roleList = [];

            $("#table-body tr").remove();

            jsonObject.forEach((element, index) => {

                const headers = Object.keys(element);

                var row = tableBody.insertRow();


                roleList[element["id"]] = element["name"];

                loopRoleList(headers, element, row);

            });

        })
        .catch(error => {
            // Handle any errors that occurred during the fetch
            console.error('Fetch error:', error);
        });

}
function loopRoleList(headers, element, row) {

    var itemIndex = 0;


    headers.forEach(header => {

        var item = element[header];
        var html = '<div class="list-group"> <ul>';


        if (header == 'allowed_clients') {

            if (element["is_admin"] == false) {
                html = '<div class="list-group"> <ul id="ac' + element["id"] + '" >';

                if (Array.isArray(item)) {
                    item.forEach(function (element) {
                        html += '<li class="list-group-item">' + element + '</li>';
                    });
                }
                else {
                    html += '<li class="list-group-item">' + item + '</li>';
                }


                html += '</ul></div>';
            }
            else
                html = "";

            row.insertCell(itemIndex).innerHTML = html;


        }
        else if (header == 'allowed_menus') {
            if (element["is_admin"] == false) {
                html = '<div class="list-group"> <ul id="aa' + element["id"] + '">';

                if (Array.isArray(item)) {
                    item.forEach(function (element) {
                        if (menuList.hasOwnProperty(element))
                            html += '<li class="list-group-item">' + menuList[element] + '</li>';

                    });
                }
                else {
                    if (menuList.hasOwnProperty(item))
                        html += '<li class="list-group-item">' + menuList[item] + '</li>';

                }

                html += '</ul></div>';
            }
            else
                html = "";

            row.insertCell(itemIndex).innerHTML = html;
        }
        else if (header == 'allowed_apps') {

            if (element["is_admin"] == false) {
                html = '<div class="list-group"> <ul id="aa' + element["id"] + '">';

                if (Array.isArray(item)) {
                    item.forEach(function (element) {
                        if (appList.hasOwnProperty(element))
                            html += '<li class="list-group-item">' + appList[element] + '</li>';
                    });
                }
                else {
                    if (appList.hasOwnProperty(item))
                        html += '<li class="list-group-item">' + appList[item] + '</li>';

                }

                html += '</ul></div>';
            }
            else
                html = "";

            row.insertCell(itemIndex).innerHTML = html;
        }
        else if (header == 'is_admin') {
            return;
        }
        else
            row.insertCell(itemIndex).textContent = element[header];

        itemIndex++;
    });

    if (element["is_admin"] == false) {
        row.insertCell(itemIndex).innerHTML = "<button class='button editButton'   onclick='edit_role(" + element["id"] + ")'>edit</button>" +
            "<button class='button deleteButton' onclick='confirm_delete(" + element["id"] + ")'>delete</button>";
    }
    else {
        row.insertCell(itemIndex).innerHTML = "";
    }
}
function get_all_clients() {
    fetch('/api/client/get-all-clients')
        .then(response => {
            // Check if the response status is OK (HTTP 200)
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }

            // Parse the response body as JSON
            return response.json();
        })
        .then(data => {
            const jsonObject = JSON.parse(data);

            jsonObject.forEach((element, index) => {

                clientList[element["client_code"]] = element["client_name"];
            });
        })
        .catch(error => {
            // Handle any errors that occurred during the fetch
            console.error('Fetch error:', error);
        });

}
async function get_all_menus() {
    fetch('/api/menu/get-all-menus')
        .then(response => {
            // Check if the response status is OK (HTTP 200)
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }

            // Parse the response body as JSON
            return response.json();
        })
        .then(data => {
            const jsonObject = JSON.parse(data);

            jsonObject.forEach((element, index) => {
                menuList[element["menu_id"]] = element["name"];
            });

        })
        .catch(error => {
            // Handle any errors that occurred during the fetch
            console.error('Fetch error:', error);
        });


}

async function get_all_apps() {
    fetch('/api/menu/get-all-apps')
        .then(response => {
            // Check if the response status is OK (HTTP 200)
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }

            // Parse the response body as JSON
            return response.json();
        })
        .then(data => {
            const jsonObject = JSON.parse(data);

            jsonObject.forEach((element, index) => {
                appList[element["menu_id"]] = element["name"];
            });

        })
        .catch(error => {
            // Handle any errors that occurred during the fetch
            console.error('Fetch error:', error);
        });


}


function refresh_clients(id) {

    var ul = $("#ac" + id + " li");
    var acList = [];

    ul.each(function () {
        acList.push($(this).text());
    });

    $("#clientList").empty();

    for (var key in clientList) {
        if (clientList.hasOwnProperty(key)) {

            var newListItem = $("<li>");
            newListItem.attr("class", "list-group-item");

            if (acList.includes(key))
                newListItem.html('<div class="form-check"><input class="form-check-input" type="checkbox" value="' + key + '" id="' + key + '" checked><span class="form-check-label">' + key + ' - ' + clientList[key] + '</span></div>');
            else
                newListItem.html('<div class="form-check"><input class="form-check-input" type="checkbox" value="' + key + '" id="' + key + '"><span class="form-check-label">' + key + ' - ' + clientList[key] + '</span></div>');
            $("#clientList").append(newListItem);
        }
    }

}


function refresh_menus(id) {

    var ul = $("#aa" + id + " li");
    var aalist = [];

    $("#menuList").empty();

    ul.each(function () {
        aalist.push($(this).text());
    });

    $("#appList").empty();

    if (id === undefined) {
        for (var key in menuList) {
            if (menuList.hasOwnProperty(key)) {

                var newListItem = $("<li>");
                newListItem.attr("class", "list-group-item");
                newListItem.html('<div class="form-check"><input class="form-check-input" type="checkbox" value="' + key + '" id="' + key + '" checked><span class="form-check-label" for="checkItem3">' + menuList[key] + '</span></div>');

                $("#menuList").append(newListItem);
            }
        }
    }
    else {
        for (var key in menuList) {
            if (menuList.hasOwnProperty(key)) {

                var newListItem = $("<li>");
                newListItem.attr("class", "list-group-item");

                if (aalist.includes(menuList[key]))
                    newListItem.html('<div class="form-check"><input class="form-check-input" type="checkbox" value="' + key + '" id="' + key + '" checked><span class="form-check-label" for="checkItem3">' + menuList[key] + '</span></div>');
                else
                    newListItem.html('<div class="form-check"><input class="form-check-input" type="checkbox" value="' + key + '" id="' + key + '"><span class="form-check-label" for="checkItem3">' + menuList[key] + '</span></div>');
                $("#menuList").append(newListItem);
            }
        }
    }

}

function refresh_apps(id) {

    var ul = $("#aa" + id + " li");
    var aalist = [];

    ul.each(function () {
        aalist.push($(this).text());
    });

    $("#appList").empty();

    for (var key in appList) {
        if (appList.hasOwnProperty(key)) {

            var newListItem = $("<li>");
            newListItem.attr("class", "list-group-item");

            if (aalist.includes(appList[key]))
                newListItem.html('<div class="form-check"><input class="form-check-input" type="checkbox" value="' + key + '" id="' + key + '" checked><span class="form-check-label" for="checkItem3">' + appList[key] + '</span></div>');
            else
                newListItem.html('<div class="form-check"><input class="form-check-input" type="checkbox" value="' + key + '" id="' + key + '"><span class="form-check-label" for="checkItem3">' + appList[key] + '</span></div>');
            $("#appList").append(newListItem);
        }
    }

}

function new_role_modal() {
    $('#mNewRoleModal').show();
    $("#saveRole").attr('onclick', 'saveRole()');

    $('#inputRole').val('');
    $("#inputRole").attr("placeholder", "Role Name");
    $('#inputRole').prop('disabled', false);

    $('#errorMsgNewRoleModal').hide('');
    $('#errorMsgNewRoleModal').val('');

    $('#modalTitle').text('New Role');


    refresh_clients();
    refresh_menus();
    refresh_apps();
}

function edit_role(id) {
    $('#mNewRoleModal').show();
    $("#saveRole").attr('onclick', 'updateRole(' + id + ')');

    $('#inputRole').val(roleList[parseInt(id)]).removeAttr('placeholder');
    $('#inputRole').prop('disabled', true);

    $('#errorMsgNewRoleModal').hide('');
    $('#errorMsgNewRoleModal').val('');

    $('#modalTitle').text('Edit Role');

    refresh_clients(id);
    refresh_menus(id);
    refresh_apps(id);
}

function delete_role(id) {

    var csrfToken = document.querySelector("input[name='__RequestVerificationToken']").value;

    var data = {
        "id": id
    };

    fetch('/api/admin/role-management/delete-role', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': csrfToken
        },
        body: JSON.stringify(data)
    })
        .then(response => response.json())
        .then(data => {
            if (data["message"] != "success") {
                error_msg('#errorMsgDeleteRoleModal', data["message"]);
            }
            else {
                $('#mConfirmDelete').hide();
                refresh_roles();
            }
        })
        .catch(error => {
            error_msg("#errorMsgDeleteRoleModal", error);
        });


}
function confirm_delete(id) {

    $('#mConfirmDelete').show();

    $('#confirmDelete').attr('onclick', 'delete_role(' + id + ')');
}

function saveRole() {

    var csrfToken = document.querySelector("input[name='__RequestVerificationToken']").value;
    var clientList = $("#clientList li input:checked");
    var appList = $("#appList li input:checked");
    var menuList = $("#menuList li input:checked");
    var client = [];
    var apps = [];
    var menus = [];

    clientList.each(function () {
        client.push($(this).val());
    });

    menuList.each(function () {
        menus.push($(this).val());
    });

    appList.each(function () {
        apps.push($(this).val());
    });

    var roleName = $("#inputRole").val();

    if (roleName.trim() === '') {
        error_msg('#errorMsgNewRoleModal', 'Please Input Role Name.');
        return;
    }

    if (client.length < 1) {
        error_msg('#errorMsgNewRoleModal', 'Please Select Allowed Clients for Role.');
        return;
    }

    if (menus.length < 1) {
        error_msg('#errorMsgNewRoleModal', 'Please Select Allowed Menus for Role.');
        return;
    }

    if (apps.length < 1) {
        error_msg('#errorMsgNewRoleModal', 'Please Select Allowed Apps for Role.');
        return;
    }


    var data = {
        "name": roleName,
        "allowed_clients": client,
        "allowed_menus": menus,
        "allowed_apps": apps
    };

    fetch('/api/admin/role-management/create-role', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': csrfToken
        },
        body: JSON.stringify(data)
    })
        .then(response => response.json())
        .then(data => {
            if (data["message"] != "success") {
                error_msg('#errorMsgNewRoleModal', data["message"]);
            }
            else {
                $('#mNewRoleModal').hide();
                refresh_roles();
            }

        })
        .catch(error => {
            error_msg('#errorMsgNewRoleModal', error);
        });



}

function updateRole(id) {

    var csrfToken = document.querySelector("input[name='__RequestVerificationToken']").value;
    var clientList = $("#clientList li input:checked");
    var appList = $("#appList li input:checked");
    var menuList = $("#menuList li input:checked");
    var client = [];
    var apps = [];
    var menus = [];

    clientList.each(function () {
        client.push($(this).val());
    });

    menuList.each(function () {
        menus.push($(this).val());
    });

    appList.each(function () {
        apps.push($(this).val());
    });


    if (client.length < 1) {
        error_msg('#errorMsgNewRoleModal', 'Please Select Allowed Clients for Role.');
        return;
    }

    if (menus.length < 1) {
        error_msg('#errorMsgNewRoleModal', 'Please Select Allowed Menus for Role.');
        return;
    }

    if (apps.length < 1) {
        error_msg('#errorMsgNewRoleModal', 'Please Select Allowed Apps for Role.');
        return;
    }

    var data = {
        "id": id,
        "allowed_clients": client,
        "allowed_menus": menus,
        "allowed_apps": apps
    };

    fetch('/api/admin/role-management/update-role', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': csrfToken
        },
        body: JSON.stringify(data)
    })
        .then(response => response.json())
        .then(data => {
            if (data["message"] != "success") {
                error_msg('#errorMsgNewRoleModal', data["message"]);
            }
            else {
                $('#mNewRoleModal').hide();
                refresh_roles();
            }

        })
        .catch(error => {
            error_msg('#errorMsgNewRoleModal', error);
        });

}


function error_msg(id, msg) {

    $(id).show();
    $(id).text(msg);

}

function delete_modal_close() {
    $('#mConfirmDelete').hide();
    $('#errorMsgDeleteRoleModal').hide();
    $('#errorMsgDeleteRoleModal').text("");

}





