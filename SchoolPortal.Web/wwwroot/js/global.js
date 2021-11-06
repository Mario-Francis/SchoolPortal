
function initializeTeachersDropdown() {
    var _select = $(".teachersdd").selectize({
        valueField: "id",
        searchField: ["email", "username", "firstName", "surname", "middleName", "phoneNumber"],
        placeholder: '- Search teacher -',
        dropdownParent: 'body',
        create: false,
        preload: 'focus',
        render: {
            option: function (item, escape) {
                return (
                    `<div class="d-flex flex-row px-3 py-2 border-top bg-white">
                        <div>
                            <div class="rounded-circle mr-3 bg-claret" style="height:36px;width:36px;padding-top:8px;">
                                <p class="m-0 f14 text-center text-white">${getInitial(item, escape)}</p>
                            </div>
                        </div>
                        <div class="flex-fill">
                            <p class="f14 font-weight-bold text-dark mt-1">${capitalize(escape(item.firstName).trim())} ${capitalize(escape(item.surname).trim())}</p>
                            <p class="f12" style="margin-top:-6px;">${escape(item.email).trim()}</p>
                        </div>
                    </div>`
                );
            },
            item: function (item, escape) {
                return (
                    `<div class="d-flex flex-row px-3 py-1">
                        <div>
                            <div class="rounded-circle mr-3 bg-claret" style="height:36px;width:36px;padding-top:8px;">
                                <p class="m-0 f14 text-center text-white">${getInitial(item, escape)}</p>
                            </div>
                        </div>
                        <div class="flex-fill">
                            <p class="f14 font-weight-bold text-dark mt-1">${capitalize(escape(item.firstName).trim())} ${capitalize(escape(item.surname).trim())}</p>
                            <p class="f12" style="margin-top:-6px;">${escape(item.email).trim()}</p>
                        </div>
                    </div>`
                );
            },
        },
        load: function (query, callback) {
            if (!query.length) return callback();
            $.ajax({
                url: $base + 'users/SearchTeachers?max=50&query=' + encodeURIComponent(query),
                type: "GET",
                error: function (err) {
                    console.log(err);
                    callback();
                },
                success: function (res) {
                    callback(res.data);
                },
            });
        },
    });
    return _select;
}

function getInitial(item, escape) {
    var l1 = item.firstName == null ? "" : escape(item.firstName.trim())[0];
    var l2 = item.surname == null ? "" : escape(item.surname.trim())[0];
    return l1 + l2;
}