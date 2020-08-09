<template>
    <div id="update-administradora">
        <h1>Atualizar Administradora</h1>

        <p><router-link :to="{ name: 'all_administradoras' }">Retornar para administradoras</router-link></p>

        <notification v-bind:notifications="notifications"></notification>

        <form v-on:submit.prevent="editAdministradora">
            <div class="form-group">
                <label name="administradora_id">ID</label>
                <input type="text" class="form-control" disabled v-model="administradora.Id" id="administradora_id">
            </div>

            <div class="form-group">
                <label name="administradora_name">Nome</label>
                <input type="text" class="form-control" v-model="administradora.NomeAdministradora" id="administradora_name" required>
            </div>

            <div class="form-group">
                <button class="btn btn-primary">Atualizar</button>
            </div>
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
                this.$http.get('http://localhost:5004/api/administradoras/save' + this.$route.params.Id).then((response) => {
                    this.administradora = response.body;
                }, (response) => {

                });
            },

            editAdministradora: function()
            {
                // Validation

                this.$http.patch('http://localhost:3000/api/administradoras/save/' + this.$route.params.Id, this.pdministradora, {
                    headers : {
                        'Content-Type' : 'application/json'
                    }
                }).then((response) => {
                    this.notifications.push({
                        type: 'success',
                        message: 'Administradora Atualizada'
                    });
                }, (response) => {
                    this.notifications.push({
                        type: 'error',
                        message: 'Administradora não Atualizada'
                    });
                });
            }
        },

        components: {
            'notification' : Notification
        }
    }
</script>
