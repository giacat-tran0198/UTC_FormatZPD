<template>
<div class='my-legend'>
    <div class='legend-title'>Colors legend</div>
    <div class='legend-scale'>
    <ul class='legend-labels'>
        <li><span style='background : linear-gradient(to right, rgb(180, 180, 180), rgb(100, 100, 98));'></span>Ignorance</li>
        <li><span style='background : linear-gradient(to right, rgb(238, 255, 0), rgb(100, 100, 0));'></span>Disability</li>
        <li><span style='background : linear-gradient(to right, rgb(255, 0, 0), rgb(100, 0, 0));'></span>Conflict</li>
        <li><span style='background : linear-gradient(to right, rgb(0, 0, 255), rgb(0, 0, 100));'></span>Ability</li>
    </ul>
    </div>
</div>
</template>

<script>
export default {
    name: 'my-legend', 
}
</script>

<style scoped>
  .my-legend .legend-title {
    text-align: left;
    margin-bottom: 8px;
    font-weight: bold;
    font-size: 90%;
    margin-left:100px; 
    }
  .my-legend .legend-scale ul {
    margin: 0;
    padding: 0;
    float: left;
    list-style: none;
    margin-left:100px; 
    }
  .my-legend .legend-scale ul li {
    display: block;
    float: left;
    width: 80px;
    margin-bottom: 6px;
    text-align: center;
    font-size: 80%;
    list-style: none;
    }
  .my-legend ul.legend-labels li span {
    display: block;
    float: left;
    height: 15px;
    width: 80px;
    }
  .my-legend .legend-source {
    font-size: 70%;
    color: #999;
    clear: both;
    
    }
  .my-legend a {
    color: #777;
    color : rgba(100, 0, 0,1); 
    color: rgb(0, 0, 100)
  
    }
</style>