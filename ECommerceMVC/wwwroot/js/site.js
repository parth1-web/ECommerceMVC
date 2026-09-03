/**
 * Gaun Ko Achar - Main Frontend Interactive Scripts
 */

document.addEventListener("DOMContentLoaded", function () {
    // ---------------------------------------------------------
    // 1. Client-side Product Live Search & Sort
    // ---------------------------------------------------------
    const searchInput = document.getElementById("productSearchInput");
    const sortSelect = document.getElementById("productSortSelect");
    const productGrid = document.getElementById("productGridContainer");
    const productCards = document.querySelectorAll(".product-item-col");
    const visibleCountLabel = document.getElementById("visibleProductsCount");

    function filterAndSortProducts() {
        if (!productGrid || !productCards.length) return;

        const query = searchInput ? searchInput.value.toLowerCase().trim() : "";
        const sortBy = sortSelect ? sortSelect.value : "default";

        let visibleCount = 0;
        const visibleArray = [];

        productCards.forEach(function (card) {
            const title = (card.getAttribute("data-name") || "").toLowerCase();
            const desc = (card.getAttribute("data-desc") || "").toLowerCase();
            const matches = !query || title.includes(query) || desc.includes(query);

            if (matches) {
                card.style.display = "";
                visibleCount++;
                visibleArray.push(card);
            } else {
                card.style.display = "none";
            }
        });

        // Sorting
        if (sortBy === "price-low" || sortBy === "price-high" || sortBy === "name-asc" || sortBy === "name-desc") {
            visibleArray.sort(function (a, b) {
                const priceA = parseFloat(a.getAttribute("data-price") || 0);
                const priceB = parseFloat(b.getAttribute("data-price") || 0);
                const nameA = (a.getAttribute("data-name") || "").toLowerCase();
                const nameB = (b.getAttribute("data-name") || "").toLowerCase();

                if (sortBy === "price-low") return priceA - priceB;
                if (sortBy === "price-high") return priceB - priceA;
                if (sortBy === "name-asc") return nameA.localeCompare(nameB);
                if (sortBy === "name-desc") return nameB.localeCompare(nameA);
                return 0;
            });

            visibleArray.forEach(function (card) {
                productGrid.appendChild(card);
            });
        }

        if (visibleCountLabel) {
            visibleCountLabel.textContent = visibleCount;
        }

        const noResultsEl = document.getElementById("noFilterResults");
        if (noResultsEl) {
            noResultsEl.classList.toggle("d-none", visibleCount > 0);
        }
    }

    if (searchInput) {
        searchInput.addEventListener("input", filterAndSortProducts);
    }
    if (sortSelect) {
        sortSelect.addEventListener("change", filterAndSortProducts);
    }

    // ---------------------------------------------------------
    // 2. Quantity Stepper Controls
    // ---------------------------------------------------------
    document.querySelectorAll(".product-qty-stepper").forEach(function (stepper) {
        const input = stepper.querySelector(".input-stepper");
        const btnMinus = stepper.querySelector(".btn-stepper-minus");
        const btnPlus = stepper.querySelector(".btn-stepper-plus");

        if (!input) return;

        if (btnMinus) {
            btnMinus.addEventListener("click", function () {
                let val = parseInt(input.value) || 1;
                const min = parseInt(input.min) || 1;
                if (val > min) {
                    input.value = val - 1;
                    input.dispatchEvent(new Event("change"));
                }
            });
        }

        if (btnPlus) {
            btnPlus.addEventListener("click", function () {
                let val = parseInt(input.value) || 1;
                const max = parseInt(input.max) || 999;
                if (val < max) {
                    input.value = val + 1;
                    input.dispatchEvent(new Event("change"));
                }
            });
        }
    });

    // ---------------------------------------------------------
    // 3. Password Visibility Toggle
    // ---------------------------------------------------------
    document.querySelectorAll(".btn-password-toggle").forEach(function (btn) {
        btn.addEventListener("click", function () {
            const targetId = btn.getAttribute("data-target");
            const input = document.getElementById(targetId);
            if (!input) return;

            const icon = btn.querySelector("i");
            if (input.type === "password") {
                input.type = "text";
                if (icon) {
                    icon.classList.remove("bi-eye");
                    icon.classList.add("bi-eye-slash");
                }
            } else {
                input.type = "password";
                if (icon) {
                    icon.classList.remove("bi-eye-slash");
                    icon.classList.add("bi-eye");
                }
            }
        });
    });

    // ---------------------------------------------------------
    // 4. Payment Method Card Selection
    // ---------------------------------------------------------
    const paymentCards = document.querySelectorAll(".payment-selector-card");
    paymentCards.forEach(function (card) {
        card.addEventListener("click", function () {
            const radio = card.querySelector('input[type="radio"]');
            if (radio) {
                radio.checked = true;
            }
            paymentCards.forEach(function (c) {
                c.classList.remove("selected");
            });
            card.classList.add("selected");
        });
    });

    // ---------------------------------------------------------
    // 5. Auto-dismissing Alerts
    // ---------------------------------------------------------
    const alerts = document.querySelectorAll(".alert.alert-dismissible");
    alerts.forEach(function (alert) {
        setTimeout(function () {
            const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            if (bsAlert) {
                bsAlert.close();
            }
        }, 6000);
    });
});

