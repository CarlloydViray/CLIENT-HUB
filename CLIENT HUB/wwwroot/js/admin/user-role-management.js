var roleList = {};
var AllowedColumns = [ "UserName", "LastName", "FirstName", "MiddleName", "Roles"];
var AdminRoleList = [];
var SuperAdminRoleList = [];

$(document).ready(function () {
    $('#filterRole').on('input', function () {
        var filterValue = $(this).val().toLowerCase();
        $('#roleList li').each(function () {
            var listItemText = $(this).text().toLowerCase();
            $(this).toggle(listItemText.includes(filterValue));
        });
    });

    $("#filterUsername").on("keyup", function () {
        var value = $(this).val().toLowerCase(); // Convert input value to lowercase
        $("#dataTable tbody tr").filter(function () {
            // Toggle the visibility of rows based on the input value in the username column
            $(this).toggle($(this).find("td:eq(0)").text().toLowerCase().indexOf(value) > -1);
        });
    });

    loadData();

});

function loadData() {
    return new Promise(resolve => {
        get_all_roles();
        setTimeout(function () {
            resolve();
        }, 1000);
    });
}

// Usage
loadData().then(function () {
    get_all_users();
});


function get_all_users() {
    fetch('/api/user-management/get-all-user-roles')
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

            jsonObject.forEach((element, index) => {

                const headers = Object.keys(element);

                if (index == 0) {
                    headers.forEach(header => {
                        if ($.inArray(header, AllowedColumns) !== -1) {
                            var headerCell = document.createElement("th");
                            headerCell.textContent = header;
                            headerRow.appendChild(headerCell);
                        }
                    });

                    var headerCell = document.createElement("th");
                    headerCell.textContent = "";
                    headerRow.appendChild(headerCell);


                    tableHead.appendChild(headerRow);
                }

                var row = tableBody.insertRow();
                var itemIndex = 0;


                loopUserRoleList(headers, element, row);

            });



        })
        .catch(error => {
            // Handle any errors that occurred during the fetch
            console.error('Fetch error:', error);
        });

}

async function refresh_users() {

    await new Promise(resolve => setTimeout(resolve, 1000));

    fetch('/api/user-management/get-all-user-roles')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }

            return response.json();
        })
        .then(data => {
            const tableHead = document.querySelector("#dataTable thead");
            const tableBody = document.getElementById("table-body");

            const jsonObject = JSON.parse(data);

            $("#table-body tr").remove();

            jsonObject.forEach((element, index) => {

                var headers = Object.keys(element);

                var row = tableBody.insertRow();

                loopUserRoleList(headers, element, row);

            });

        })
        .catch(error => {
            console.error('Fetch error:', error);
        });

}
function loopUserRoleList(headers, element, row) {

    var itemIndex = 0;

    var isAdmin = false;
    var isSuperAdmin = false;

    headers.forEach(header => {
        if ($.inArray(header, AllowedColumns) !== -1) {

            var item = element[header];
            var html = '<div class="list-group"> <ul>';


            if (header == 'Roles') {

                html = '<div class="list-group"> <ul id="ar' + element["UserName"] + '" >';

                if (Array.isArray(item)) {
                    item.forEach(function (element) {
                        if (roleList.hasOwnProperty(element))
                            html += '<li class="list-group-item">' + roleList[element] + '</li>';
                    });
                }
                else {
                    if (roleList.hasOwnProperty(item))
                        html += '<li class="list-group-item">' + roleList[item] + '</li>';

                }


                html += '</ul></div>';

                row.insertCell(itemIndex).innerHTML = html;


            }
            else if (header == 'UserName')
            {
                


                if (Array.isArray(element["Roles"])) {
                    element["Roles"].forEach(function (role) {
                        if (AdminRoleList.includes(role))
                            isAdmin = true;

                        if (SuperAdminRoleList.includes(role))
                            isSuperAdmin = true;

                    });

                    if (isAdmin)
                        row.insertCell(itemIndex).innerHTML = element[header] + "  <span class='admin-badge'>Admin</span>";
                    else if (isSuperAdmin)
                        row.insertCell(itemIndex).innerHTML = element[header] + "  <span class='admin-badge'>Super Admin</span>";
                    else
                        row.insertCell(itemIndex).textContent = element[header];
                }
                else {
                    if ($.inArray(AdminRoleList, element["Roles"]) !== -1)
                        row.insertCell(itemIndex).innerHTML = element[header] + "  <span class='admin-badge'>Admin</span>";
                    else
                        row.insertCell(itemIndex).textContent = element[header];

                }

                
            }
            else
                row.insertCell(itemIndex).textContent = element[header];

            itemIndex++;
        }
    });

    if (isSuperAdmin)
        row.insertCell(itemIndex).innerHTML = "";
    else
        row.insertCell(itemIndex).innerHTML = "<button class='button editButton'   onclick='edit_role(\"" + element["UserName"].trim() + "\")'>edit</button>";
}
function get_all_roles() {
    fetch('/api/admin/role-management/get-all-user-roles')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            const jsonObject = JSON.parse(data);

            jsonObject.forEach((element, index) => {

                roleList[element["Id"]] = element["Name"];

                if (element["IsAdmin"] == true) {

                    if (element["Name"] == "super admin")
                        SuperAdminRoleList.push(element["Id"].toString());
                    else
                        AdminRoleList.push(element["Id"].toString());
                }
                    
            });


        })
        .catch(error => {
            console.error('Fetch error:', error);
        });

}


function refresh_roles(id) {

    var ul = $("#ar" + id + " li");
    var arList = [];

    ul.each(function () {
        arList.push($(this).text());
    });

    $("#roleList").empty();

    for (var key in roleList) {
        if (roleList.hasOwnProperty(key)) {

            var newListItem = $("<li>");
            newListItem.attr("class", "list-group-item");

            if(roleList[key] == "admin")
                newListItem.attr("class", "list-group-item list-group-item-admin");

            if (roleList[key] == "super admin")
                continue;

            if (arList.includes(roleList[key]))
                newListItem.html('<div class="form-check"><input class="form-check-input" type="checkbox"  value="' + key + '" id="' + key + '" checked><label class="form-check-label" for="' + key + '">' + roleList[key] + '</label></div>');
            else
                newListItem.html('<div class="form-check"><input class="form-check-input" type="checkbox"  value="' + key + '" id="' + key + '"><label class="form-check-label" for="' + key + '">' + roleList[key] + '</label></div>');
            $("#roleList").append(newListItem);
        }
    }

}

function edit_role(id) {
    $('#mEditRoleModal').show();
    $("#updateRole").attr('onclick', 'updateRole(\"' + id + '\")');


    $('#errorMsgNewRoleModal').hide('');
    $('#errorMsgNewRoleModal').val('');


    refresh_roles(id);

}

function updateRole(id) {

    var csrfToken = document.querySelector("input[name='__RequestVerificationToken']").value;
    var rList = $("#roleList li input:checked");
    var roles = [];


    rList.each(function () {
        roles.push($(this).val());
    });


    if (roles.length < 1) {
        error_msg('#errorMsgNewRoleModal', 'Please Select Roles for ' + id + '.');
        return;
    }


    const roleJSON = JSON.stringify(roles);

    var data = {
        "Username": id,
        "Roles": roles
    };

    fetch('/api/user-management/update-user-role', {
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
                $('#mEditRoleModal').hide();
                refresh_users();
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






