// ============================================================
// Admin User Management - Client-Side Interactions
// ============================================================

(function () {
    'use strict';

    let currentUserId = null;

    // ============================================================
    // EDIT USER
    // ============================================================

    window.editUser = async function (userId) {
        currentUserId = userId;

        // Show modal
        const modal = new bootstrap.Modal(document.getElementById('editUserModal'));
        modal.show();

        // Load user data
        try {
            const response = await fetch(`/admin/user/api/${userId}`);
            const result = await response.json();

            if (result.success && result.user) {
                populateEditModal(result.user);
            } else {
                showError('Failed to load user data: ' + (result.error || 'Unknown error'));
            }
        } catch (error) {
            console.error('Error loading user:', error);
            showError('Failed to load user data. Please try again.');
        }
    };

    function populateEditModal(user) {
        const modalBody = document.getElementById('editUserModalBody');

        modalBody.innerHTML = `
            <form id="editUserForm">
                <input type="hidden" id="editUserId" value="${user.userId}" />

                <div class="row mb-3">
                    <div class="col-md-6">
                        <label class="form-label fw-bold">Username</label>
                        <input type="text" class="form-control" id="editUserName" value="${user.userName}" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label fw-bold">Email</label>
                        <input type="email" class="form-control" id="editEmail" value="${user.email}" />
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-md-6">
                        <label class="form-label fw-bold">First Name</label>
                        <input type="text" class="form-control" id="editFirstName" value="${user.firstName || ''}" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label fw-bold">Last Name</label>
                        <input type="text" class="form-control" id="editLastName" value="${user.lastName || ''}" />
                    </div>
                </div>

                <div class="mb-3">
                    <label class="form-label fw-bold">Roles</label>
                    <div class="row">
                        ${generateRoleCheckboxes(user.roles)}
                    </div>
                </div>

                <div class="mb-3">
                    <label class="form-label fw-bold">Notes</label>
                    <textarea class="form-control" id="editNotes" rows="3">${user.notes || ''}</textarea>
                </div>

                <div class="form-check mb-3">
                    <input type="checkbox" class="form-check-input" id="editIsActive" ${user.isActive ? 'checked' : ''} />
                    <label class="form-check-label" for="editIsActive">
                        User is Active
                    </label>
                </div>

                <div class="d-flex justify-content-end gap-2">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                    <button type="submit" class="btn btn-primary">
                        <i class="fas fa-save me-2"></i>Save Changes
                    </button>
                </div>
            </form>
        `;

        // Attach submit handler
        document.getElementById('editUserForm').addEventListener('submit', handleEditSubmit);
    }

    function generateRoleCheckboxes(rolesString) {
        const availableRoles = [
            { id: 'super-admin', name: 'Super Admin' },
            { id: 'company-manager', name: 'Company Manager' },
            { id: 'user-manager', name: 'User Manager' },
            { id: 'monitor', name: 'Monitor' },
            { id: 'viewer', name: 'Viewer' }
        ];

        const userRoles = rolesString.split(',').map(r => r.trim().toLowerCase());

        return availableRoles.map(role => {
            const isChecked = userRoles.includes(role.name.toLowerCase());
            return `
                <div class="col-md-6 mb-2">
                    <div class="form-check">
                        <input type="checkbox" class="form-check-input edit-role-checkbox" 
                               value="${role.name}" id="edit-role-${role.id}" ${isChecked ? 'checked' : ''} />
                        <label class="form-check-label" for="edit-role-${role.id}">
                            ${role.name}
                        </label>
                    </div>
                </div>
            `;
        }).join('');
    }

    async function handleEditSubmit(e) {
        e.preventDefault();

        const userId = document.getElementById('editUserId').value;
        const userName = document.getElementById('editUserName').value;
        const email = document.getElementById('editEmail').value;
        const firstName = document.getElementById('editFirstName').value;
        const lastName = document.getElementById('editLastName').value;
        const isActive = document.getElementById('editIsActive').checked;
        const notes = document.getElementById('editNotes').value;

        // Get selected roles
        const roleCheckboxes = document.querySelectorAll('.edit-role-checkbox:checked');
        const roles = Array.from(roleCheckboxes).map(cb => cb.value);

        const updateData = {
            userName,
            email,
            firstName,
            lastName,
            isActive,
            roles,
            notes
        };

        try {
            const response = await fetch(`/admin/user/api/${userId}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(updateData)
            });

            const result = await response.json();

            if (result.success) {
                showSuccess('User updated successfully!');

                // Close modal
                const modal = bootstrap.Modal.getInstance(document.getElementById('editUserModal'));
                modal.hide();

                // Refresh page to show updated data
                setTimeout(() => {
                    window.location.reload();
                }, 1000);
            } else {
                showError('Failed to update user: ' + (result.error || 'Unknown error'));
            }
        } catch (error) {
            console.error('Error updating user:', error);
            showError('Failed to update user. Please try again.');
        }
    }

    // ============================================================
    // TOGGLE USER STATUS
    // ============================================================

    window.toggleUserStatus = async function (userId, currentStatus) {
        const newStatus = !currentStatus;
        const action = newStatus ? 'activate' : 'deactivate';

        if (!confirm(`Are you sure you want to ${action} this user?`)) {
            return;
        }

        try {
            const response = await fetch(`/admin/user/api/${userId}/toggle-status`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ isActive: newStatus })
            });

            const result = await response.json();

            if (result.success) {
                showSuccess(`User ${action}d successfully!`);

                // Update the row UI
                updateUserRow(userId, result.isActive);
            } else {
                showError(`Failed to ${action} user: ` + (result.error || 'Unknown error'));
            }
        } catch (error) {
            console.error('Error toggling user status:', error);
            showError(`Failed to ${action} user. Please try again.`);
        }
    };

    function updateUserRow(userId, isActive) {
        const row = document.querySelector(`tr[data-user-id="${userId}"]`);
        if (!row) return;

        // Update status badge
        const statusCell = row.querySelector('td:nth-child(5)');
        if (statusCell) {
            if (isActive) {
                statusCell.innerHTML = `
                    <span class="badge bg-success">
                        <i class="fas fa-check-circle me-1"></i>Active
                    </span>
                `;
            } else {
                statusCell.innerHTML = `
                    <span class="badge bg-secondary">
                        <i class="fas fa-times-circle me-1"></i>Inactive
                    </span>
                `;
            }
        }

        // Update toggle button
        const toggleBtn = row.querySelector('button[onclick*="toggleUserStatus"]');
        if (toggleBtn) {
            toggleBtn.className = isActive ? 'btn btn-outline-warning' : 'btn btn-outline-success';
            toggleBtn.innerHTML = `<i class="fas ${isActive ? 'fa-pause' : 'fa-play'}"></i>`;
            toggleBtn.setAttribute('onclick', `toggleUserStatus('${userId}', ${isActive})`);
        }
    }

    // ============================================================
    // DELETE USER
    // ============================================================

    window.deleteUser = function (userId, userName) {
        currentUserId = userId;

        // Populate modal
        document.getElementById('deleteUserName').textContent = userName;

        // Show modal
        const modal = new bootstrap.Modal(document.getElementById('deleteUserModal'));
        modal.show();
    };

    // Attach confirm delete handler
    document.addEventListener('DOMContentLoaded', function () {
        const confirmBtn = document.getElementById('confirmDeleteBtn');
        if (confirmBtn) {
            confirmBtn.addEventListener('click', handleConfirmDelete);
        }
    });

    async function handleConfirmDelete() {
        if (!currentUserId) return;

        const confirmBtn = document.getElementById('confirmDeleteBtn');
        const originalText = confirmBtn.innerHTML;

        // Show loading state
        confirmBtn.disabled = true;
        confirmBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Deleting...';

        try {
            const response = await fetch(`/admin/user/api/${currentUserId}`, {
                method: 'DELETE'
            });

            const result = await response.json();

            if (result.success) {
                showSuccess('User deleted successfully!');

                // Close modal
                const modal = bootstrap.Modal.getInstance(document.getElementById('deleteUserModal'));
                modal.hide();

                // Remove row from table or refresh page
                const row = document.querySelector(`tr[data-user-id="${currentUserId}"]`);
                if (row) {
                    row.remove();
                }

                // If no more users, show empty message
                const tbody = document.querySelector('#users-table tbody');
                if (tbody && tbody.children.length === 0) {
                    window.location.reload();
                }
            } else {
                showError('Failed to delete user: ' + (result.error || 'Unknown error'));
                confirmBtn.disabled = false;
                confirmBtn.innerHTML = originalText;
            }
        } catch (error) {
            console.error('Error deleting user:', error);
            showError('Failed to delete user. Please try again.');
            confirmBtn.disabled = false;
            confirmBtn.innerHTML = originalText;
        }
    }

    // ============================================================
    // NOTIFICATIONS
    // ============================================================

    function showSuccess(message) {
        showToast(message, 'success');
    }

    function showError(message) {
        showToast(message, 'danger');
    }

    function showToast(message, type) {
        // Remove existing toasts
        const existingToasts = document.querySelectorAll('.user-manage-toast');
        existingToasts.forEach(toast => toast.remove());

        // Create toast
        const toast = document.createElement('div');
        toast.className = `alert alert-${type} alert-dismissible fade show position-fixed user-manage-toast`;
        toast.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
        toast.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;

        document.body.appendChild(toast);

        // Auto-remove after 5 seconds
        setTimeout(() => {
            toast.remove();
        }, 5000);
    }

})();
