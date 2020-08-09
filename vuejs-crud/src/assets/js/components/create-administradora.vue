<template>
    <div id="createadministradora">
        <h1>Criar Administradora</h1>

        <p><router-link :to="{ name: 'all_administradoras' }">Retornar para administradoras</router-link></p>

        <notification v-bind:notifications="notifications"></notification>

        <form v-on:submit.prevent="addAdministradora">
            <div class="form-group">
                <label name="administradora_id">ID</label>
                <input type="text" class="form-control" disabled v-model="administradora.Id" id="administradora_id">
            </div>

            <div class="form-group">
                <label name="administradora_name">Nome</label>
                <input type="text" class="form-control" v-model="administradora.NomeAdministradora" id="administradora_name" required>
            </div>


            <div class="form-group">
                <button class="btn btn-primary">Criar</button>
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

        methods: {
            addAdministradora: function()
            {
                // Validation

                this.$http.post('http://localhost:5004/api/administradoras/save', this.administradora, {
                    headers : {
                        'Content-Type' : 'application/json'
                    }
                }).then((response) => {
                    this.notifications.push({
                        type: 'success',
                        message: 'Administradora criado com Sucesso'
                    });
                }, (response) => {
                    this.notifications.push({
                        type: 'error',
                        message: 'Administradora não criada'
                    });
                });
            }
        },

        components: {
            'notification' : Notification
        }
    }
</script>
