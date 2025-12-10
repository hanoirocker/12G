- La contemplacion esta bloqueada mientras se muestre la corrutina del fallback de interaccion
- Vehiculos en escena. 2 Autos regulares + Helicoptero (solo sonido y animacion) y Police Car 2.
- Police Car 2 incluido con sonido de choque y efecto de particulas pero no disparado por evento aun, a usar en "Headaches".

TOFIX - urgente:
- Grabar sonidos para cuando come la pizza, toma un plato para servir la pizza, deposita la basura en el tacho, abre y cierra el tacho de basura.
- Cambiar posicion de tecla de luz de garage.
- Cambiar posicion de bolsa.
- Agregar efecto al coger objeto que se dirija hacia la camara?.
- Cambiar "dumpster" por garbage can en texto en ingles al limpiar el ave.
- Agregar opcion para sacar plato desde la pila de platos del drawer de la cocina.
- Desactivar colliders en drawers de la cocina (los de arriba) y de la puerta del microondas.
- Agrendar el collider puerta de heladera y de microondas (microondas para abrir cerrar puerta, y para colocar el plato tambien)
- Reducir brillo del plano del microondas.
- Frenar emision en dos sentidos de la television.
- Deshabilitar TODOS los Renderers components en los examinable objects. Actualmente al dejar de exa
- Agregar "H" en la palabra "Johnny ..." en el contemplable del cuadro de skate grande.
- Agregar collider de uso de PC a la CPU en si ademas de la pantalla.
- Las fuentes siguen quedando modificadas en pitch.
- No esta frenando la musica Hounting Sound luego de comer la pizza. Revisar.
- Agregar cajas bajo la escalera, o algo.
- El televisor de fernandez brilla con luz naranja fuertemente, no la que debe. Quizas se introdujo este error al modificar el material emision del microondas original.
- Faltan textos en ingles para los dialogos de FirstContact y el texto en ingles para la mancha de sangre.
- Hay que agregar iconos con el numero del canal del Walkie Talkie activo por el momento. A futuro tambien cambiar el Control Canvas.
- Cambiar texto en ingles y espaniol sobre la direccion de Maria Acosta que dice 1-3-4 y poner 1-3-5.
- Durante los dialogos, deshabilitar el zoom.

TODO - próximo:
- Definitivamente se tiene que indicar en el menu de pausa lo que el jugador debe ir haciendo. Frente a cualquier distraccion se pierde facilmente el hilo del juego.
- Comenzar a trabajar en evento Headaches.
- Actualizar textos en puertas principales y objetos contemplables al final de la escena de Evening. Por ahora se han dejado de trabajar los SO's.

TODO - despues:
- SISTEMA GUARDADO POR CHECKPOINTS: Dividir las escenas evening y night en escenas mas chicas con menos eventos, y configurar cada escena con los objetos necesarios para usarlas como punto de restauracion de juego.
- Implementar cambios de Video del Settings Menu
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.

TOCHECK: