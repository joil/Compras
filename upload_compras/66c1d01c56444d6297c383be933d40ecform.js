Ext.onReady(function(){
var formulario = new Ext.FormPanel({
url: 'mi-archivo-php.php',
renderTo: 'contenedor',
title: 'Mi primera ventana',
width: 250,
items: [{
xtype: 'textfield',
fieldLabel: 'Titulo',
name: 'titulo'
},{
xtype: 'textfield',
fieldLabel: 'Descripcion',
name: 'descripcion'
},{
xtype: 'datefield',
fieldLabel: 'fecha',
name: 'fecha'
}]
});
});
