import Vue from 'vue'

import VueRouter from 'vue-router'
Vue.use(VueRouter)

import VueResource from 'vue-resource';
Vue.use(VueResource);

import App from './App.vue'

const AllAdministradoras = require('./assets/js/components/all-administradoras.vue');
const CreateAdministradora = require('./assets/js/components/create-administradora.vue');
const EditAdministradora = require('./assets/js/components/edit-administradora.vue');
const DeleteAdministradora = require('./assets/js/components/delete-administradora.vue');

const routes = [
    {
        name: 'all_administradoras',
        path: '/',
        component: AllAdministradoras
    },
    {
        name: 'create_administradora',
        path: '/administradoras/create',
        component: CreateAdministradora
    },
    {
        name: 'edit_administradora',
        path: '/administradoras/edit/:Id',
        component: EditAdministradora
    },
    {
        name: 'delete_administradora',
        path: '/administradoras/delete/:id',
        component: DeleteAdministradora
    }
];
var router = new VueRouter({ routes: routes, mode: 'history' });
new Vue(Vue.util.extend({ router }, App)).$mount('#app');