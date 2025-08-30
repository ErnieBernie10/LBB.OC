// modal.js
document.addEventListener('alpine:init', () => {
    Alpine.data('Modal', () => ({
        isOpen: false,
        animationDuration: 400,

        init() {
            this.$watch('isOpen', (value) => {
                if (value) {
                    this.handleScrollbar();
                    document.documentElement.classList.add('modal-is-open', 'modal-is-opening');
                    setTimeout(() => {
                        document.documentElement.classList.remove('modal-is-opening');
                    }, this.animationDuration);
                }
            });

            // Handle click outside
            document.addEventListener('click', (event) => {
                if (!this.isOpen) return;
                const dialog = this.$el.querySelector('dialog');
                const modalContent = this.$el.querySelector('article');

                // Ignore if clicking on the open button or if the modal isn't open
                if (event.target.closest('button[\\@click="open"]')) return;

                // Check if click is outside both dialog and article
                if (dialog && modalContent &&
                    !modalContent.contains(event.target) &&
                    !event.target.closest('button[\\@click="close"]')) {
                    this.close();
                }
            });

            // Handle Escape key
            document.addEventListener('keydown', (event) => {
                if (event.key === 'Escape' && this.isOpen) {
                    this.close();
                }
            });
        },

        open() {
            this.isOpen = true;
        },

        close() {
            document.documentElement.classList.add('modal-is-closing');
            setTimeout(() => {
                this.isOpen = false;
                document.documentElement.classList.remove('modal-is-closing', 'modal-is-open');
                document.documentElement.style.removeProperty('--pico-scrollbar-width');
            }, this.animationDuration);
        },

        handleScrollbar() {
            const scrollbarWidth = window.innerWidth - document.documentElement.clientWidth;
            if (scrollbarWidth) {
                document.documentElement.style.setProperty('--pico-scrollbar-width', `${scrollbarWidth}px`);
            }
        }
    }));
});
