function ajaxErrorHandler(req, status, err, { callback = null }) {
    let res = req.responseJSON;
    if (req.status == 401) {
        notify(res.message, 'danger', "Unauthorized");
    } else if (req.status == 400) {
        let eItems = '';
        if (res.errorItems != null) {
            eItems = '<ul>';
            res.errorItems.forEach((v, i) => {
                eItems += `<li>${i + 1}. ${v}</li>`;
            });
            eItems += '</ul>';
        }
        notify(res.message + eItems, 'danger', "Validation Error");
    } else if (req.status == 500) {
        notify(res.message, 'danger');
        console.log(res.errorDetail)
    } else {
        notify('Something went wrong while submitting your request. Please refresh your browser and try again.', 'danger');
        console.error(req);
    }
    if (callback != null)
        callback();
}