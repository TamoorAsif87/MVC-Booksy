function setupPasswordToggle(toggleBtnId, inputId, iconId) {
    const toggleBtn = document.getElementById(toggleBtnId);
    const passwordInput = document.getElementById(inputId);
    const icon = document.getElementById(iconId);

    if (!toggleBtn || !passwordInput || !icon) return;

    toggleBtn.addEventListener('click', function () {
        const isPassword = passwordInput.type === 'password';
        passwordInput.type = isPassword ? 'text' : 'password';
        icon.classList.toggle('bi-eye', isPassword);
        icon.classList.toggle('bi-eye-slash', ! isPassword);
    });
}

function showToast(type, heading, message) {
    $.toast({
        heading: heading,
        text: message,
        icon: type,
        showHideTransition: 'slide',
        position: 'top-right',
        loaderBg: '#333'
    });
}



$('.add-to-cart-btn').on('click', function (e) {
    e.preventDefault();

    const $btn = $(this);

    const bookData = {
        bookId: $btn.data('book-id'),
        bookCover: $btn.data('book-cover'),
        name: $btn.attr('data-name'),
        author: $btn.data('author'),
        categoryName: $btn.data('category-name'),
        price: parseFloat($btn.data('price')),
        quantity: parseInt($btn.data('quantity'))
    };

    addToCart(bookData);
});



function addToCart(data) {

    //console.log(data)
    $('#global-spinner').removeClass("d-none");

    $.ajax({
        url: '/cart/add',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            if (response.success) {

                //remove the spinner
                $("#global-spinner").addClass("d-none");

                //show success message
                showToast('success', 'Added to Cart', `${data.name} added to cart.`);

                //update the mini cart
                refreshMiniCart();

                //update  cart
                refreshCart();

                    
            } else {
                $("#global-spinner").addClass("d-none");
                showToast('error', 'Error', response.message);
            }
        },
        error: function () {
            showToast('error', 'Error', 'Unexpected error occurred.');
        }
    });
}


function refreshMiniCart() {
    $.get('/cart/mini-view', function (html) {
        //console.log(html)
        $('#mini-cart').html(html);
    });
}



function updateCart(bookId, quantity) {
    $('#global-spinner').removeClass("d-none");
    $.ajax({
        url: '/cart/update',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ bookId, quantity }),
        success: function (response) {
            if (response.success) {

                // Remove the spinner
                $('#global-spinner').addClass("d-none");

                // Update the cart view with the new quantity
                refreshCart();

                // Update the mini cart with the new quantity
                refreshMiniCart();
                // Show success message
                showToast('success', 'Cart Updated', 'Cart quantity updated successfully.');
            }
        },
        error: function () {
            alert('Failed to update cart quantity.');
        }
    });
}

function refreshCart() {
    $.get("/cart/update/ajax", function (html) {
        
        $('#cart').html(html);
    });
}

function removeFromCart(bookId) {
    $('#global-spinner').removeClass("d-none");
    $.ajax({
        url: '/cart/remove',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ bookId }),
        success: function (response) {
            if (response.success) {

                // Remove the spinner
                $('#global-spinner').addClass("d-none");

                // Update the cart view with the new quantity
                refreshCart();

                // Update the mini cart with the new quantity
                refreshMiniCart();

                //remove bought together if items are zero

                if (response.itemsCount == 0) {
                    $('#cart-books').html("");
                }

                // Show success message
                showToast('success', 'Cart Updated', 'Item remove successfully successfully.');
            }
        },
        error: function () {
            alert('Failed to remove Item.');
        }
    });
}


//rewview creation
document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('reviewForm');
    const messageDiv = document.getElementById('reviewMessage');

    if (!form) return;

    form.addEventListener('submit', function (e) {
        e.preventDefault();

        const formData = new FormData(form);
        const encodedData = new URLSearchParams(formData).toString();

        fetch('/reviews/create', {
            method: 'POST',
            headers: {
                'RequestVerificationToken': form.querySelector('input[name="__RequestVerificationToken"]').value,
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: encodedData
        })
            .then(response => response.json())
            .then(data => {
                // Handle unauthenticated user
                if (!data.success && data.message === "Please login First") {
                    const returnUrl = encodeURIComponent(window.location.pathname + window.location.search);
                    window.location.href = `/account/login?returnurl=${returnUrl}`;
                    return;
                }

                // Handle validation errors
                if (!data.success && data.errors) {
                    messageDiv.innerHTML = `<div class="alert alert-danger" style="white-space:pre-line">${data.errors}</div>`;
                    return;
                }

                // Success - reset form and update reviews
                if (data.success) {
                    messageDiv.innerHTML = `<div class="alert alert-success">${data.message}</div>`;
                    form.reset();

                    // Update the reviews list
                    if (data.result) {
                        refreshReviewSection(data.result)
                    }
                }
            })
            .catch(error => {
                messageDiv.innerHTML = `<div class="alert alert-danger">${error.message}</div>`;
            });
    });
});


function refreshReviewSection(bookId) {
    $.get(`/book/${bookId}`, function (html) {
        $('#bookReviews').html(html); 
    });
}
