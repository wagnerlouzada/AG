<template>
    <div id="delete-administradora">
        <h1>Excluir Administradora {{ administradora.NomeAdministradora }}</h1>

        <p><router-link :to="{ name: 'all_administradoras' }">Retornar para administradoras</router-link></p>

        <notification v-bind:notifications="notifications"></notification>

        <form v-on:submit.prevent="deleteAdministradora">
            <p><button class="btn btn-danger">Excluir Administradora</button></p>
        </form>
    </div>
</template>

<script>
    import Notification from './notifications.vue';

    export default{
        data(){
            return{
                administradora:{},
                notifications:[]
            }
        },

        created: function(){
            this.getAdministradora();
        },

        methods: {
            getAdministradora: function()
            {
                this.$http.get('http://localhost:5004/api/pdministradoras/' + this.$route.params.id).then((response) => {
                    this.administradora = response.body;
                }, (response) => {

                });
            },

            deleteAdministradora: function()
            {
                this.$http.delete('http://localhost:3000/api/administradora/delete/' + this.$route.params.id, this.pdministradora, {
                    headers : {
                        'Content-Type' : 'application/json'
                    }
                }).then((response) => {
                    this.$router.push({name: 'all_administradoras'})
                }, (response) => {
                    this.notifications.push({
                        type: 'danger',
                        message: 'Administradora não foi Excluida'
                    });
                });
            }
        },

        components: {
            'notification' : Notification
        }
    }
</script>
