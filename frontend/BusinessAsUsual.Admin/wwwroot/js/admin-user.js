// ============================================================
// Admin User Wizard Navigation & Validation
// ============================================================

(function () {
    'use strict';

    let currentStep = 1;
    const totalSteps = 3;

    // ============================================================
    // INITIALIZATION
    // ============================================================

    document.addEventListener('DOMContentLoaded', function () {
        initializeWizard();
        attachEventListeners();
    });

    // ============================================================
    // WIZARD INITIALIZATION
    // ============================================================

    function initializeWizard() {
        showStep(1);
        updateNavigationButtons();
    }

    // ============================================================
    // EVENT LISTENERS
    // ============================================================

    function attachEventListeners() {
        // Next button
        const btnNext = document.getElementById('btnNext');
        if (btnNext) {
            btnNext.addEventListener('click', handleNext);
        }

        // Previous button
        const btnPrevious = document.getElementById('btnPrevious');
        if (btnPrevious) {
            btnPrevious.addEventListener('click', handlePrevious);
        }

        // Role checkboxes - update warning
        const roleCheckboxes = document.querySelectorAll('.role-checkbox');
        roleCheckboxes.forEach(checkbox => {
            checkbox.addEventListener('change', updateRoleWarning);
        });

        // Role cards - click anywhere to toggle checkbox
        const roleCards = document.querySelectorAll('.role-card');
        roleCards.forEach(card => {
            card.addEventListener('click', function (e) {
                if (e.target.tagName !== 'INPUT') {
                    const checkbox = card.querySelector('.role-checkbox');
                    if (checkbox) {
                        checkbox.checked = !checkbox.checked;
                        updateRoleWarning();
                    }
                }
            });
        });

        // Form submission - validate before submit
        const form = document.getElementById('addUserForm');
        if (form) {
            form.addEventListener('submit', handleSubmit);
        }
    }

    // ============================================================
    // NAVIGATION HANDLERS
    // ============================================================

    function handleNext() {
        // Clear any previous validation errors
        clearValidationErrors();

        if (validateCurrentStep()) {
            if (currentStep < totalSteps) {
                currentStep++;
                showStep(currentStep);
                updateNavigationButtons();

                // If moving to review step, populate review data
                if (currentStep === 3) {
                    populateReviewStep();
                }
            }
        }
    }

    function handlePrevious() {
        if (currentStep > 1) {
            currentStep--;
            showStep(currentStep);
            updateNavigationButtons();
        }
    }

    function handleSubmit(e) {
        // Final validation before submission
        if (!validateCurrentStep()) {
            e.preventDefault();
            return false;
        }

        // Show loading state
        const btnSubmit = document.getElementById('btnSubmit');
        if (btnSubmit) {
            btnSubmit.disabled = true;
            btnSubmit.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Creating User...';
        }

        return true;
    }

    // ============================================================
    // STEP VISIBILITY
    // ============================================================

    function showStep(step) {
        // Hide all panels
        const panels = document.querySelectorAll('.wizard-panel');
        panels.forEach(panel => {
            panel.style.display = 'none';
        });

        // Show current panel
        const currentPanel = document.querySelector(`.wizard-panel[data-step="${step}"]`);
        if (currentPanel) {
            currentPanel.style.display = 'block';
        }

        // Update progress indicator
        updateProgressIndicator(step);
    }

    function updateProgressIndicator(step) {
        const steps = document.querySelectorAll('.wizard-step');
        steps.forEach((stepElement, index) => {
            const stepNumber = index + 1;
            const circle = stepElement.querySelector('.wizard-step-circle');

            if (stepNumber < step) {
                // Completed steps
                circle.classList.add('completed');
                circle.classList.remove('active');
                stepElement.classList.remove('active');
            } else if (stepNumber === step) {
                // Current step
                circle.classList.add('active');
                circle.classList.remove('completed');
                stepElement.classList.add('active');
            } else {
                // Future steps
                circle.classList.remove('active', 'completed');
                stepElement.classList.remove('active');
            }
        });
    }

    function updateNavigationButtons() {
        const btnPrevious = document.getElementById('btnPrevious');
        const btnNext = document.getElementById('btnNext');
        const btnSubmit = document.getElementById('btnSubmit');

        // Show/hide Previous button
        if (btnPrevious) {
            btnPrevious.style.display = currentStep > 1 ? 'block' : 'none';
        }

        // Show Next or Submit button
        if (currentStep === totalSteps) {
            if (btnNext) btnNext.style.display = 'none';
            if (btnSubmit) btnSubmit.style.display = 'block';
        } else {
            if (btnNext) btnNext.style.display = 'block';
            if (btnSubmit) btnSubmit.style.display = 'none';
        }
    }

    // ============================================================
    // VALIDATION
    // ============================================================

    function validateCurrentStep() {
        let isValid = true;

        if (currentStep === 1) {
            // Validate Basic Info
            isValid = validateBasicInfo();
        } else if (currentStep === 2) {
            // Role assignment is optional, so always valid
            // But we show a warning if no roles selected
            updateRoleWarning();
        }

        return isValid;
    }

    function validateBasicInfo() {
        let isValid = true;
        const errors = [];

        // Username
        const username = document.getElementById('UserName');
        if (username && !username.value.trim()) {
            errors.push('Username is required.');
            isValid = false;
        } else if (username && username.value.trim().length < 3) {
            errors.push('Username must be at least 3 characters.');
            isValid = false;
        } else if (username && !/^[a-zA-Z0-9_\-\.]+$/.test(username.value.trim())) {
            errors.push('Username can only contain letters, numbers, underscores, hyphens, and periods.');
            isValid = false;
        }

        // Email
        const email = document.getElementById('Email');
        if (email && !email.value.trim()) {
            errors.push('Email is required.');
            isValid = false;
        } else if (email && !isValidEmail(email.value.trim())) {
            errors.push('Email format is invalid.');
            isValid = false;
        }

        // Password
        const password = document.getElementById('Password');
        if (password && !password.value) {
            errors.push('Password is required.');
            isValid = false;
        } else if (password && password.value.length < 8) {
            errors.push('Password must be at least 8 characters.');
            isValid = false;
        }

        // Confirm Password
        const confirmPassword = document.getElementById('ConfirmPassword');
        if (confirmPassword && !confirmPassword.value) {
            errors.push('Password confirmation is required.');
            isValid = false;
        } else if (password && confirmPassword && password.value !== confirmPassword.value) {
            errors.push('Passwords do not match.');
            isValid = false;
        }

        // Show errors if any
        if (!isValid) {
            showValidationErrors(errors);
        }

        return isValid;
    }

    function isValidEmail(email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }

    function showValidationErrors(errors) {
        // Create or update alert
        let alertDiv = document.querySelector('.validation-alert');
        if (!alertDiv) {
            alertDiv = document.createElement('div');
            alertDiv.className = 'alert alert-danger validation-alert';
            const form = document.getElementById('addUserForm');
            if (form) {
                form.insertBefore(alertDiv, form.firstChild);
            }
        }

        alertDiv.innerHTML = '<strong>Please fix the following errors:</strong><ul>' +
            errors.map(err => `<li>${err}</li>`).join('') +
            '</ul>';

        // Scroll to top
        alertDiv.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
    }

    function clearValidationErrors() {
        // Remove validation alert if it exists
        const alertDiv = document.querySelector('.validation-alert');
        if (alertDiv) {
            alertDiv.remove();
        }
    }

    // ============================================================
    // ROLE WARNING
    // ============================================================

    function updateRoleWarning() {
        const roleCheckboxes = document.querySelectorAll('.role-checkbox:checked');
        const warning = document.getElementById('noRolesWarning');

        if (warning) {
            warning.style.display = roleCheckboxes.length === 0 ? 'block' : 'none';
        }
    }

    // ============================================================
    // REVIEW STEP POPULATION
    // ============================================================

    function populateReviewStep() {
        // Username
        const username = document.getElementById('UserName');
        const reviewUsername = document.getElementById('review-username');
        if (reviewUsername && username) {
            reviewUsername.textContent = username.value || '-';
        }

        // Email
        const email = document.getElementById('Email');
        const reviewEmail = document.getElementById('review-email');
        if (reviewEmail && email) {
            reviewEmail.textContent = email.value || '-';
        }

        // First Name
        const firstName = document.getElementById('FirstName');
        const reviewFirstName = document.getElementById('review-firstname');
        if (reviewFirstName && firstName) {
            reviewFirstName.textContent = firstName.value || '-';
        }

        // Last Name
        const lastName = document.getElementById('LastName');
        const reviewLastName = document.getElementById('review-lastname');
        if (reviewLastName && lastName) {
            reviewLastName.textContent = lastName.value || '-';
        }

        // Status
        const isActive = document.getElementById('IsActive');
        const reviewStatus = document.getElementById('review-status');
        if (reviewStatus && isActive) {
            reviewStatus.innerHTML = isActive.checked
                ? '<span class="badge bg-success">Active</span>'
                : '<span class="badge bg-secondary">Inactive</span>';
        }

        // Roles
        const roleCheckboxes = document.querySelectorAll('.role-checkbox:checked');
        const reviewRoles = document.getElementById('review-roles');
        if (reviewRoles) {
            if (roleCheckboxes.length > 0) {
                const roleLabels = Array.from(roleCheckboxes).map(cb => {
                    const label = document.querySelector(`label[for="${cb.id}"]`);
                    return label ? label.textContent.trim() : cb.value;
                });
                reviewRoles.innerHTML = roleLabels.map(role => `<span class="badge bg-primary me-1 mb-1">${role}</span>`).join('');
            } else {
                reviewRoles.innerHTML = '<span class="text-muted">No roles assigned</span>';
            }
        }

        // Notes
        const notes = document.getElementById('Notes');
        const reviewNotes = document.getElementById('review-notes');
        const reviewNotesSection = document.getElementById('review-notes-section');
        if (reviewNotes && notes && reviewNotesSection) {
            if (notes.value.trim()) {
                reviewNotes.textContent = notes.value;
                reviewNotesSection.style.display = 'block';
            } else {
                reviewNotesSection.style.display = 'none';
            }
        }
    }

})();
