$(() => {
    $('#sendBtn').on('click', (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        try {
            let form = $("form")[0];
            if (validateForm(form)) {
                let username = $.trim($('#username').val());
                let type = $.trim($('#type').val());

                if (username == '' || type=='') {
                    notify('All fields are required', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<span class="fa fa-circle-notch fa-spin"></span> Sending reset link...');
                    let url = $base + `auth/SendPasswordResetLink?email=${username}&type=${type}`;
                    $.ajax({
                        type: 'POST',
                        url: url,
                        success: (response) => {
                            if (response.isSuccess) {
                                notify(response.message +'.', 'success');
                                form.reset();
                            } else {
                                notify(response.message, 'danger');
                            }
                            btn.html('<span class="fa fa-sign-in-alt"></span>&nbsp; Send password reset link');
                            $('fieldset').prop('disabled', false);
                        },
                        error: (req, status, err) => {
                            ajaxErrorHandler(req, status, err, {
                                callback: () => {
                                    btn.html('<span class="fa fa-sign-in-alt"></span>&nbsp; Send password reset link');
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
            btn.html('<span class="fa fa-sign-in-alt"></span>&nbsp; Send password reset link');
            $('fieldset').prop('disabled', false);
        }
    });
});