- Se modificaron los colliders de todos los objetos examinables en la casa, aumentando su tamaño.

TOCHECK:

TOFIX:
- Si cancelamos la examinación de un objeto, y luego presionamos escape para abrir el menu de pause, se vuelve a ejecutar el sonido de examinacion.

- Si habilitamos el Walkie Talkie mediante un checkpoint asset, se rompe al mostrarlo o intentar cambiar el canal debido a que no genera una estructura de canales. // Esto sucede cuando no existe extructura de audios de canales para el evento en particular //

TODO - próximo:

- HEADACHES: actualmente se terminó el final de la primera parte, pero queda aún agregar lo que sucedería entre el final de Noises y el comienzo de Headaches donde choca el auto de la policia. Simon debería intentar contactarse con Mica luego de haber escuchado la conversación policial, pero sin lograrlo. Se podría mencionar el dolor de cabeza que le ocasiona el walkie talkie y los aparatos electrónicos. Quizas luego de eso podriamos prender y apagar aparatos aleatoriamente? (microondas, radio, luces de la casa)
- Agregar sonido de mareo y caida del jugador al suelo, dependiendo del lugar donde caiga debe sonar distinto (3 materiales: carpet, wood, mosaic)
- Agregar sonidos stereo en loop para ambientación de cada habitación dentro de la casa. Se pueden disparar gracias al House Areas handler creado recientemente. 
- Componer distintos audios según intensidad de Electric Feel. La idea es que sean clips conformados por el sonido actual (loopeable), luego un clip que incluya el layer anterior mas un fade in del nuevo layer de audio (Play one shot), y finalmente el nuevo audio (loopeable).

TODO - despues:

- Agregar sonidos para cuando come la pizza.
- Agregar sonidos al tomar escoba y bolsas de basura.
- Aumentar ganancia de tracks 12G - Door - Close (Hard) 2 y 12G - Door - Close (Hard) 1 ??
- Agregar animación de primera activación de linterna para Unwired? (MAYBE)
- Agregar sonido para animación "Night - Stand Up".
- Agregar sonido para animación "Night - Unwired - Look For Flashlight" de FlashlightVC
- Agregar sonido de duda "Hum?" para transición de cámara a Focus enemy en Visions.

- Agregar cajas bajo la escalera, o algo.

- Actualizar textos en puertas principales y objetos contemplables al final de la escena de Evening. Por ahora se han dejado de trabajar los SO's.
- Implementar cambios de Video del Settings Menu
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.

