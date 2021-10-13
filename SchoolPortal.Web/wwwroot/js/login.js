$(() => {
    $(".owl-carousel").owlCarousel({
        loop: true,
        items: 6,
        margin: 8,
        nav: false,
        dots:true,
        autoplay: true,
        autoplayTimeout: 3500,
        autoplayHoverPause: true,
        responsive: {
            0: {
                items: 2,
            },
            900: {
                items: 3,
            },
            1200: {
                items: 4,
            },
            1300: {
                items: 5,
            },
            1400: {
                items: 6,
                autoplay: false,
            }
        }
    });

    $('#submitBtn').on('click', (e) => {
        e.preventDefault();
        let btn = $(e.currentTarget);
        try {
            let form = $("form")[0];
            if (validateForm(form)) {
                let username = $.trim($('#username').val());
                let password = $('#password').val();
                let returnUrl = $('#returnUrl').val();

                if (username == '' || password == '') {
                    notify('Username and password fields are required', 'warning');
                } else {
                    $('fieldset').prop('disabled', true);
                    btn.html('<i class="fa fa-circle-notch fa-spin"></i> Logging in...');
                    let url = $base + 'auth/login';
                    let data = {
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
                                setCookie('.Ecoperformance.AuthToken', response.data);
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
                            btn.html('<i class="fa fa-sign-in-alt"></i>&nbsp; Login');
                            $('fieldset').prop('disabled', false);
                        },
                        error: (req, status, err) => {
                            let res = req.responseJSON;
                            if (req.status == 401) {
                                notify(res.message, 'danger', "Unauthorized");
                            } else if (req.status == 400) {
                                let eItems = '<ul>';
                                res.errorItems.forEach((v, i) => {
                                    eItems += `<li>${i + 1}. ${v}</li>`;
                                });
                                eItems += '</ul>';
                                notify(res.message + eItems, 'danger', "Validation Error");
                            } else if (req.status == 500) {
                                notify(res.message, 'danger');
                                console.log(res.errorDetail)
                            } else {
                                notify('Something went wrong while submitting your request. Please refresh your browser and try again.', 'danger');
                                console.error(req);
                            }
                            btn.html('<i class="fa fa-sign-in-alt"></i>&nbsp; Login');
                            $('fieldset').prop('disabled', false);
                        }
                    });
                }
            }
        } catch (ex) {
            console.error(ex);
            notify(ex.message, 'danger');
            btn.html('<i class="fa fa-sign-in-alt"></i>&nbsp; Login');
            $('fieldset').prop('disabled', false);
        }
    });
});