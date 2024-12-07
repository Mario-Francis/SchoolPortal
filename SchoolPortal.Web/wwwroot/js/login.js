$(() => {
   

    $('#loginBtn').on('click', (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        try {
            let form = $("form")[0];
            if (validateForm(form)) {
                let type = $.trim($('#type').val());
                let username = $.trim($('#username').val());
                let password = $('#password').val();
                let returnUrl = $('#returnUrl').val();

                if (username == '' || password == '' || type=='') {
                    notify('Username and password fields are required', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<span class="fa fa-circle-notch fa-spin"></span> Logging in...');
                    let url = $base + 'auth/login';
                    let data = {
                        type,
                        username,
                        password
                    };
                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: data,
                        success: (response) => {
                            if (response.isSuccess) {
                                // save token
                                //setCookie('.Ecoperformance.AuthToken', response.data);
                                notify(response.message + '<br /><i class="fa fa-circle-notch fa-spin"></i> Redirecting...', 'success');
                                form.reset();
                                setTimeout(() => {
                                    if (returnUrl == null || returnUrl == '') {
                                        location.replace($base + 'Dashboard');
                                    } else {
                                        location.replace(returnUrl);
                                    }
                                }, 2000);

                            } else {
                                notify(response.message, 'danger');
                            }
                            btn.html('<span class="fa fa-sign-in-alt"></span>&nbsp; Login');
                            $('fieldset').prop('disabled', false);
                        },
                        error: (req, status, err) => {
                            ajaxErrorHandler(req, status, err, {
                                callback: () => {
                                    btn.html('<span class="fa fa-sign-in-alt"></span>&nbsp; Login');
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
            btn.html('<span class="fa fa-sign-in-alt"></span>&nbsp; Login');
            $('fieldset').prop('disabled', false);
        }
    });
});