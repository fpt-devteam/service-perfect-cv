// Admin Panel JavaScript

(function() {
    'use strict';

    // Menu Toggle
    const menuToggle = document.getElementById('menuToggle');
    const sidebar = document.querySelector('.sidebar');

    if (menuToggle && sidebar) {
        menuToggle.addEventListener('click', function() {
            sidebar.classList.toggle('open');
        });

        // Close sidebar when clicking outside on mobile
        document.addEventListener('click', function(event) {
            const isClickInside = sidebar.contains(event.target) || menuToggle.contains(event.target);
            if (!isClickInside && window.innerWidth <= 1024) {
                sidebar.classList.remove('open');
            }
        });
    }

    // User Menu Dropdown
    const userMenuBtn = document.getElementById('userMenuBtn');
    const userDropdown = document.getElementById('userDropdown');

    if (userMenuBtn && userDropdown) {
        userMenuBtn.addEventListener('click', function(e) {
            e.stopPropagation();
            userMenuBtn.classList.toggle('active');
            userDropdown.classList.toggle('show');
        });

        // Close dropdown when clicking outside
        document.addEventListener('click', function(event) {
            if (!userMenuBtn.contains(event.target) && !userDropdown.contains(event.target)) {
                userMenuBtn.classList.remove('active');
                userDropdown.classList.remove('show');
            }
        });
    }

    // Close sidebar when window resizes above mobile breakpoint
    window.addEventListener('resize', function() {
        if (window.innerWidth > 1024 && sidebar) {
            sidebar.classList.remove('open');
        }
    });

    // Form validation helper
    function validateForm(form) {
        const inputs = form.querySelectorAll('[required]');
        let isValid = true;

        inputs.forEach(input => {
            if (!input.value.trim()) {
                isValid = false;
                input.classList.add('is-invalid');
            } else {
                input.classList.remove('is-invalid');
            }
        });

        return isValid;
    }

    // Add form validation to all forms with class 'needs-validation'
    const forms = document.querySelectorAll('.needs-validation');
    forms.forEach(form => {
        form.addEventListener('submit', function(event) {
            if (!validateForm(form)) {
                event.preventDefault();
                event.stopPropagation();
            }
        });
    });

    // Auto-hide alerts after 5 seconds
    const alerts = document.querySelectorAll('.alert:not(.alert-permanent)');
    alerts.forEach(alert => {
        setTimeout(() => {
            alert.style.opacity = '0';
            setTimeout(() => alert.remove(), 300);
        }, 5000);
    });

    // Smooth scroll to top button (if exists)
    const scrollTopBtn = document.getElementById('scrollTopBtn');
    if (scrollTopBtn) {
        window.addEventListener('scroll', function() {
            if (window.pageYOffset > 300) {
                scrollTopBtn.classList.add('show');
            } else {
                scrollTopBtn.classList.remove('show');
            }
        });

        scrollTopBtn.addEventListener('click', function() {
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });
    }

    // Data table helpers
    window.perfectCV = {
        // Confirm before delete
        confirmDelete: function(message) {
            return confirm(message || 'Are you sure you want to delete this item?');
        },

        // Show loading spinner
        showLoading: function() {
            const loader = document.createElement('div');
            loader.id = 'global-loader';
            loader.innerHTML = '<div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div>';
            loader.style.cssText = 'position:fixed;top:0;left:0;right:0;bottom:0;background:rgba(0,0,0,0.5);display:flex;align-items:center;justify-content:center;z-index:9999;';
            document.body.appendChild(loader);
        },

        // Hide loading spinner
        hideLoading: function() {
            const loader = document.getElementById('global-loader');
            if (loader) {
                loader.remove();
            }
        },

        // Show toast notification
        showToast: function(message, type = 'info') {
            const toast = document.createElement('div');
            toast.className = `alert alert-${type} toast-notification`;
            toast.textContent = message;
            toast.style.cssText = 'position:fixed;top:20px;right:20px;z-index:9999;min-width:300px;animation:slideInRight 0.3s ease;';
            document.body.appendChild(toast);

            setTimeout(() => {
                toast.style.animation = 'slideOutRight 0.3s ease';
                setTimeout(() => toast.remove(), 300);
            }, 3000);
        }
    };

    // Bulk Selection for Tables
    window.bulkSelection = {
        selectedIds: new Set(),

        // Initialize bulk selection for a table
        init: function(tableId) {
            const table = document.getElementById(tableId);
            if (!table) return;

            // Add select all checkbox to header
            const selectAllCheckbox = table.querySelector('.select-all-checkbox');
            if (selectAllCheckbox) {
                selectAllCheckbox.addEventListener('change', (e) => {
                    this.toggleAll(e.target.checked, tableId);
                });
            }

            // Add event listeners to all row checkboxes
            const rowCheckboxes = table.querySelectorAll('.row-checkbox');
            rowCheckboxes.forEach(checkbox => {
                checkbox.addEventListener('change', (e) => {
                    this.toggleRow(e.target.value, e.target.checked);
                    this.updateBulkActions();
                });
            });

            this.updateBulkActions();
        },

        // Toggle all rows
        toggleAll: function(checked, tableId) {
            const table = document.getElementById(tableId);
            const rowCheckboxes = table.querySelectorAll('.row-checkbox');
            
            rowCheckboxes.forEach(checkbox => {
                checkbox.checked = checked;
                this.toggleRow(checkbox.value, checked);
            });

            this.updateBulkActions();
        },

        // Toggle individual row
        toggleRow: function(id, checked) {
            if (checked) {
                this.selectedIds.add(id);
            } else {
                this.selectedIds.delete(id);
            }
        },

        // Update bulk actions UI
        updateBulkActions: function() {
            const bulkActionsBar = document.getElementById('bulkActionsBar');
            const selectedCount = document.getElementById('selectedCount');
            
            if (bulkActionsBar && selectedCount) {
                if (this.selectedIds.size > 0) {
                    bulkActionsBar.classList.remove('d-none');
                    selectedCount.textContent = this.selectedIds.size;
                } else {
                    bulkActionsBar.classList.add('d-none');
                }
            }
        },

        // Clear all selections
        clearSelection: function() {
            this.selectedIds.clear();
            document.querySelectorAll('.row-checkbox, .select-all-checkbox').forEach(cb => {
                cb.checked = false;
            });
            this.updateBulkActions();
        },

        // Get selected IDs as array
        getSelectedIds: function() {
            return Array.from(this.selectedIds);
        },

        // Perform bulk action
        performAction: function(action, confirmMessage) {
            if (this.selectedIds.size === 0) {
                window.perfectCV.showToast('Please select at least one item', 'warning');
                return false;
            }

            if (confirmMessage && !confirm(confirmMessage)) {
                return false;
            }

            return true;
        }
    };

    console.log('Perfect CV Admin Panel initialized with bulk selection');
})();

