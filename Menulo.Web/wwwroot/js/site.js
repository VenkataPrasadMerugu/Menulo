const applyTheme = (theme) => {
    document.documentElement.setAttribute("data-theme", theme);
    document.querySelectorAll(".js-theme-toggle").forEach((button) => {
        button.setAttribute("aria-pressed", String(theme === "dark"));
        const label = button.querySelector(".theme-toggle-label");
        if (label) {
            label.textContent = theme === "dark" ? "Light mode" : "Dark mode";
        }
    });
};

const storedTheme = localStorage.getItem("menulo-theme");
if (storedTheme === "light" || storedTheme === "dark") {
    applyTheme(storedTheme);
}

document.querySelectorAll(".js-theme-toggle").forEach((button) => {
    button.addEventListener("click", () => {
        const currentTheme = document.documentElement.getAttribute("data-theme") === "dark" ? "dark" : "light";
        const nextTheme = currentTheme === "dark" ? "light" : "dark";
        localStorage.setItem("menulo-theme", nextTheme);
        applyTheme(nextTheme);
    });
});

document.querySelectorAll(".js-copy-link").forEach((button) => {
    button.addEventListener("click", async () => {
        const text = button.dataset.copyText;
        if (!text) {
            return;
        }

        await navigator.clipboard.writeText(text);
        const originalText = button.textContent;
        button.textContent = "Copied";

        window.setTimeout(() => {
            button.textContent = originalText;
        }, 1500);
    });
});

document.querySelectorAll(".js-color-sync").forEach((input) => {
    input.addEventListener("input", () => {
        const target = document.querySelector(input.dataset.target);
        if (target) {
            target.value = input.value.toUpperCase();
        }
    });
});

document.querySelectorAll(".js-color-text").forEach((input) => {
    input.addEventListener("input", () => {
        const colorInput = input.parentElement?.querySelector(".js-color-sync");
        if (colorInput && /^#[0-9A-Fa-f]{6}$/.test(input.value)) {
            colorInput.value = input.value;
        }
    });
});

document.querySelectorAll(".js-palette-option").forEach((option) => {
    option.addEventListener("change", () => {
        const primary = option.dataset.primary;
        const secondary = option.dataset.secondary;

        const primaryPicker = document.getElementById("PrimaryColor");
        const secondaryPicker = document.getElementById("SecondaryColor");
        const primaryText = document.getElementById("PrimaryColorText");
        const secondaryText = document.getElementById("SecondaryColorText");

        if (primaryPicker && primaryText && primary) {
            primaryPicker.value = primary;
            primaryText.value = primary;
        }

        if (secondaryPicker && secondaryText && secondary) {
            secondaryPicker.value = secondary;
            secondaryText.value = secondary;
        }
    });
});

document.querySelectorAll("[data-check-all]").forEach((toggle) => {
    toggle.addEventListener("change", () => {
        const group = toggle.getAttribute("data-check-all");
        if (!group) {
            return;
        }

        document.querySelectorAll(`[data-check-group="${group}"]`).forEach((checkbox) => {
            checkbox.checked = toggle.checked;
        });
    });
});

document.querySelectorAll(".js-auto-alert").forEach((alertElement) => {
    const timeout = Number.parseInt(alertElement.dataset.timeout ?? "3500", 10);
    if (Number.isNaN(timeout) || !window.bootstrap?.Alert) {
        return;
    }

    window.setTimeout(() => {
        if (alertElement.isConnected) {
            window.bootstrap.Alert.getOrCreateInstance(alertElement).close();
        }
    }, timeout);
});

document.querySelectorAll(".js-open-phone-preview").forEach((button) => {
    button.addEventListener("click", () => {
        const modal = document.getElementById("phonePreviewModal");
        const frame = modal?.querySelector(".js-phone-preview-frame");
        const previewSrc = button.dataset.previewSrc;

        if (!frame || !previewSrc) {
            return;
        }

        frame.setAttribute("src", previewSrc);
    });
});

const phonePreviewModal = document.getElementById("phonePreviewModal");
if (phonePreviewModal) {
    phonePreviewModal.addEventListener("hidden.bs.modal", () => {
        const frame = phonePreviewModal.querySelector(".js-phone-preview-frame");
        if (frame) {
            frame.removeAttribute("src");
        }
    });
}

const menuFilterState = {
    search: "",
    foodType: "all",
    availability: "all",
    category: "all"
};

const applyMenuFilters = () => {
    document.querySelectorAll("[data-menu-item]").forEach((item) => {
        const searchText = (item.dataset.search || "").toLowerCase();
        const matchesSearch = !menuFilterState.search || searchText.includes(menuFilterState.search);
        const matchesFoodType = menuFilterState.foodType === "all" || item.dataset.foodType === menuFilterState.foodType;
        const matchesAvailability = menuFilterState.availability === "all" || item.dataset.availability === menuFilterState.availability;
        const matchesCategory = menuFilterState.category === "all" || item.dataset.category === menuFilterState.category;
        item.hidden = !(matchesSearch && matchesFoodType && matchesAvailability && matchesCategory);
    });
};

document.querySelectorAll(".js-menu-filter").forEach((button) => {
    button.addEventListener("click", () => {
        const filterGroup = button.dataset.filterGroup;
        const filterValue = button.dataset.filterValue;
        if (!filterGroup || !filterValue) {
            return;
        }

        const currentValue = menuFilterState[filterGroup];
        menuFilterState[filterGroup] = currentValue === filterValue ? "all" : filterValue;

        document.querySelectorAll(`.js-menu-filter[data-filter-group="${filterGroup}"]`).forEach((chip) => {
            const chipValue = chip.dataset.filterValue;
            chip.classList.toggle("active", menuFilterState[filterGroup] === "all" ? chipValue === "all" : chipValue === menuFilterState[filterGroup]);
        });

        applyMenuFilters();
    });
});

document.querySelectorAll(".js-menu-search").forEach((input) => {
    input.addEventListener("input", () => {
        menuFilterState.search = input.value.trim().toLowerCase();
        applyMenuFilters();
    });
});
