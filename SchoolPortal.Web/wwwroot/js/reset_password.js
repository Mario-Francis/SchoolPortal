$(() => {
    $('#resetBtn').on('click', (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        try {
            let form = $("form")[0];
            if (validateForm(form)) {
                let userId = $('#userId').val();
                let userType = $('#type').val();
                let password = $.trim($('#password').val());
                let cpassword = $.trim($('#cpassword').val());

                if (password == '' || cpassword == '') {
                    notify('Passwords are required', 'warning');
                } else if (password != cpassword) {
                    notify("Passwords don't match", 'warning');
                }else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<span class="fa fa-circle-notch fa-spin"></span> Resetting password...');
                    let url = $base + 'auth/ResetPassword';
                    let data = {
                        userType,
                        userId,
                        password,
                        confirmPassword: cpassword
                    }
                    $.ajax({
                        type: 'POST',
                        url: url,
                        data,
                        success: (response) => {
                            if (response.isSuccess) {
                                notify(response.message + '<br /><i class="fa fa-circle-notch fa-spin"></i> Redirecting...', 'success');
                                form.reset();
                                setTimeout(() => {
                                    location.replace($base + 'Auth');
                                }, 2000);
                            } else {
                                notify(response.message, 'danger');
                            }
                            btn.html('<span class="fa fa-sign-in-alt"></span>&nbsp; Reset password');
                            $('fieldset').prop('disabled', false);
                        },
                        error: (req, status, err) => {
                            ajaxErrorHandler(req, status, err, {
                                callback: () => {
                                    btn.html('<span class="fa fa-sign-in-alt"></span>&nbsp; Reset password');
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
            btn.html('<span class="fa fa-sign-in-alt"></span>&nbsp; Reset password');
            $('fieldset').prop('disabled', false);
        }
    });
});