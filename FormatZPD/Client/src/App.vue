<template>
  <div id="app">
  <template>
   <div class="hidden">
      <vs-sidebar
        absolute
        hover-expand
        reduce
        v-model="active"
        open
        >
        <vs-sidebar-item id="home">
          <template #icon>
            <font-awesome-icon icon="user" />
          </template>
          <vs-sidebar-item to="/">Students</vs-sidebar-item>
        </vs-sidebar-item>
        <vs-sidebar-item id="knowledges">
          <template #icon>
            <font-awesome-icon icon="network-wired" />  
          </template>
          <vs-sidebar-item to="/knowledges">Knowledges</vs-sidebar-item>
        </vs-sidebar-item>
      </vs-sidebar>
    </div>
  </template>
   <router-view/>
    <notifications group="foo"  position = "bottom right"/>

  </div>
</template>

<script>


export default {
 name: 'app',
 data:() => ({
        active: 'home',
 })
}

</script>


<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

#nav {
  padding: 30px;
}

#nav a {
  font-weight: bold;
  color: #2c3e50;
}

#nav a.router-link-exact-active {
  color: #42b983;
}

</style>
