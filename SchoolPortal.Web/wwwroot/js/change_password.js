$(() => {
    $('#changeBtn').on('click', (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        try {
            let form = $("form")[0];
            if (validateForm(form)) {
                let password = $.trim($('#password').val());
                let newPassword = $.trim($('#npassword').val());
                let confirmNewPassword = $.trim($('#cnpassword').val());

                if (password == '' || newPassword == '' || confirmNewPassword=='') {
                    notify('Passwords are required', 'warning');
                } else if (newPassword != confirmNewPassword) {
                    notify("New passwords don't match", 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<span class="fa fa-circle-notch fa-spin"></span> Changing password...');
                    let url = $base + 'profile/ChangePassword';
                    let data = {
                        password,
                        newPassword,
                        confirmNewPassword
                    }
                    $.ajax({
                        type: 'POST',
                        url: url,
                        data,
                        success: (response) => {
                            if (response.isSuccess) {
                                notify(response.message + '.', 'success');
                                form.reset();
                            } else {
                                notify(response.message, 'danger');
                            }
                            btn.html('<i class="fa fa-check-circle"></i> &nbsp;Submit');
                            $('fieldset').prop('disabled', false);
                        },
                        error: (req, status, err) => {
                            ajaxErrorHandler(req, status, err, {
                                callback: () => {
                                    btn.html('<i class="fa fa-check-circle"></i> &nbsp;Submit');
                                    $('fieldset').prop('disabled', false);
                                }
                            });
                        }
                    });
                }
            }
        } catch (ex) {
            console.error(ex);
            notify(ex.message, 'danger');
            btn.html('<i class="fa fa-check-circle"></i> &nbsp;Submit');
            $('fieldset').prop('disabled', false);
        }
    });
});