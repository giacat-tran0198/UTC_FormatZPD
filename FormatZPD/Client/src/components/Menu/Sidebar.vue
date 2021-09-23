<template>
    <div class="sidebar">
        <div class="sidebar-backdrop" @click="closeSidebarPanel" v-if="isPanelOpen"></div>
        <transition name="slide">
            <div v-if="isPanelOpen"
                 class="sidebar-panel">
                 <div class ="sidebar-header">Menu</div>
                <slot></slot>
            </div>
        </transition>
    </div>
</template>
<script>


    export default {
        methods: {
            closeSidebarPanel: function(){
             this.$store.commit('toggleNav')
            }
        },
        computed: {
            isPanelOpen() {
                return this.$store.state.isNavOpen
            }
        }
    }
</script>

<style>
    .slide-enter-active,
    .slide-leave-active
    {
        transition: transform 0.2s ease;
    }

    .slide-enter,
    .slide-leave-to {
        transform: translateX(-100%);
        transition: all 150ms ease-in 0s
    }

    .sidebar-backdrop {
        background-color: rgba(250, 250, 250, 0.89);
        width: 100vw;
        height: 100vh;
        position: fixed;
        top: 0;
        left: 0;
        cursor: pointer;
    }

    .sidebar-panel {
        overflow-y: auto;
        background-color: #1850c083;
        position: fixed;
        left: 0;
        top: 0;
        height: 100vh;
        z-index: 999;
        padding: 3rem 20px 2rem 20px;
        width: 300px;
    }
    .sidebar-header {
        font-size: 22px;
        color : white;
        text-align: center;
        line-height: 50px; 
        background : #dadcdd77; 
        user-select : none; 
        border-bottom: 2px solid grey; 
    }
</style>