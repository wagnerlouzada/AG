<template>
    <div id="all-administradora">
        <h1>Todas Administradoras</h1>

        <p><router-link :to="{ name: 'create_administradora' }" class="btn btn-primary">Nova Administradora</router-link></p>

        <div class="form-group">
            <input type="text" name="search" v-model="administradoraSearch" placeholder="Procura Administradora" class="form-control" v-on:keyup="searchadministradora">
        </div>

        <table class="table table-hover">
            <thead>
            <tr>
                <td>ID</td>
                <td>Nome da Administradora</td>
            </tr>
            </thead>

            <tbody>
                <tr v-for="administradora in administradoras">
                    <td>{{ administradora.Id }}</td>
                    <td>{{ administradora.NomeAdministradora }}</td>
                    <td>
                        <router-link :to="{name: 'edit_administradora', params: { Id: administradora.Id }}" class="btn btn-primary">Editar</router-link>
                        <router-link :to="{name: 'delete_administradora', params: { Id: administradora.Id }}" class="btn btn-danger">Excluir</router-link>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</template>

<script>

    export default{
        data(){
            return{
                administradoras: [],
                originaladministradora: [],
                administradoraSearch: ''
            }
        },

        created: function()
        {
            this.fetchadministradoraData();
        },

        methods: {
            fetchadministradoraData: function()
            {
                this.$http.get('http://localhost:5004/api/administradoras').then((response) => {
                    this.administradoras = response.body;
					
                    this.originaladministradora = this.administradoras;
                }, (response) => {

                });
            },

            searchadministradora: function()
            {
                if(this.administradoraSearch == '')
                {
                    this.administradoras = this.originaladministradora;
                    return;
                }

                var searchedadministradora = [];
                for(var i = 0; i < this.originaladministradora.length; i++)
                {
                    var administradoraName = this.originaladministradora[i]['NomeAdministradora'].toLowerCase();
                    if(administradoraName.indexOf(this.administradoraSearch.toLowerCase()) >= 0)
                    {
                        searchedadministradora.push(this.originaladministradora[i]);
                    }
                }

                this.administradoras = searchedadministradora;
            }
        }
    }
</script>
