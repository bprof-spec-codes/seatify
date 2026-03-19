import { Component, ElementRef, ViewChild, AfterViewInit, OnDestroy } from '@angular/core';
import * as THREE from 'three';

@Component({
  selector: 'app-seat-map-display',
  standalone: false,
  template: `<div #canvasContainer class="w-100 h-100" style="cursor: grab;"></div>`,
  styleUrl: './seat-map-display.component.sass'
})
export class SeatMapDisplayComponent implements AfterViewInit, OnDestroy {
  @ViewChild('canvasContainer', { static: true }) canvasContainer!: ElementRef<HTMLDivElement>;

  private scene!: THREE.Scene;
  private camera!: THREE.PerspectiveCamera;
  private renderer!: THREE.WebGLRenderer;

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.initThreeJS();
      this.createFairytaleAmphitheater();
      this.renderer.render(this.scene, this.camera);
    }, 100);
  }

  ngOnDestroy(): void {
    if (this.renderer) {
      this.renderer.dispose();
    }
    window.removeEventListener('resize', this.onWindowResize);
  }

  private initThreeJS(): void {
    const container = this.canvasContainer.nativeElement;

    this.scene = new THREE.Scene();

    this.camera = new THREE.PerspectiveCamera(50, container.clientWidth / container.clientHeight, 0.1, 1000);
    this.camera.position.set(17, 38, 30);
    this.camera.lookAt(-3, 2, -25);

    this.renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true, powerPreference: "high-performance", precision: "highp" });
    this.renderer.setClearColor(0x000000, 0); 
    this.renderer.setSize(container.clientWidth, container.clientHeight);
    this.renderer.setPixelRatio(window.devicePixelRatio);
    this.renderer.shadowMap.enabled = true;
    this.renderer.shadowMap.type = THREE.PCFSoftShadowMap;
    this.renderer.toneMapping = THREE.ACESFilmicToneMapping;
    container.appendChild(this.renderer.domElement);

    const ambientLight = new THREE.AmbientLight(0xffffff, 0.8);
    this.scene.add(ambientLight);

    const sunLight = new THREE.DirectionalLight(0xffffff, 1.0);
    sunLight.position.set(-20, 50, 20);
    sunLight.castShadow = true;
    sunLight.shadow.mapSize.width = 2048;
    sunLight.shadow.mapSize.height = 2048;
    sunLight.shadow.camera.near = 0.5;
    sunLight.shadow.camera.far = 150;
    sunLight.shadow.bias = -0.001;
    this.scene.add(sunLight);

    const fillLight = new THREE.DirectionalLight(0xbfdbfe, 0.6);
    fillLight.position.set(25, 20, -20);
    this.scene.add(fillLight);

    const shadowPlane = new THREE.Mesh(
      new THREE.PlaneGeometry(200, 200),
      new THREE.ShadowMaterial({ opacity: 0.06 })
    );
    shadowPlane.rotation.x = -Math.PI / 2;
    shadowPlane.position.y = 0;
    shadowPlane.receiveShadow = true;
    this.scene.add(shadowPlane);

    window.addEventListener('resize', this.onWindowResize, false);
  }

  private createRoundedSeatGeometry(width: number, height: number, depth: number, radius: number): THREE.ExtrudeGeometry {
    const shape = new THREE.Shape();
    const x = -width / 2;
    const y = -height / 2;
    shape.absarc(x + width - radius, y + height - radius, radius, 0, Math.PI / 2, false);
    shape.absarc(x + radius, y + height - radius, radius, Math.PI / 2, Math.PI, false);
    shape.absarc(x + radius, y + radius, radius, Math.PI, Math.PI * 1.5, false);
    shape.absarc(x + width - radius, y + radius, radius, Math.PI * 1.5, Math.PI * 2, false);
    
    const geometry = new THREE.ExtrudeGeometry(shape, { 
      depth: depth, 
      bevelEnabled: true,
      bevelSegments: 4, 
      steps: 2, 
      bevelSize: 0.05, 
      bevelThickness: 0.05, 
      curveSegments: 24 
    });
    geometry.center(); 
    return geometry;
  }

  private createFairytaleAmphitheater(): void {
    const stageGeo = new THREE.CylinderGeometry(15, 15, 0.8, 80);
    const stageMat = new THREE.MeshPhysicalMaterial({ color: 0xf8fafc, roughness: 0.2, metalness: 0.1, clearcoat: 0.5 }); 
    const stage = new THREE.Mesh(stageGeo, stageMat);
    stage.position.set(0, 0.4, 0);
    stage.receiveShadow = true;
    stage.castShadow = true;
    this.scene.add(stage);

    const seatGeometry = this.createRoundedSeatGeometry(4.5, 1.5, 3, 0.4);
    
    const materialGreen = new THREE.MeshPhysicalMaterial({ color: 0x34d399, roughness: 0.3, metalness: 0.1, clearcoat: 0.4 }); 
    const materialRoyalBlue = new THREE.MeshPhysicalMaterial({ color: 0x38bdf8, roughness: 0.3, metalness: 0.1, clearcoat: 0.4 }); 
    const materialAmber = new THREE.MeshPhysicalMaterial({ color: 0xfb923c, roughness: 0.4, metalness: 0.1, clearcoat: 0.2 });

    const totalRows = 8; 
    const baseRadius = 19; 

    for (let row = 0; row < totalRows; row++) {
      const radius = baseRadius + (row * 5); 
      const height = 1.6 + (row * 2); 
      
      const seatSpacing = 6.0; 
      const maxSpreadAngle = Math.PI * 1; 
      
      const angleStep = seatSpacing / radius;

      let seatsInRow = Math.floor(maxSpreadAngle / angleStep);
      
      if (row % 2 === 0) {
        if (seatsInRow % 2 === 0) seatsInRow--; 
      } else {
        if (seatsInRow % 2 !== 0) seatsInRow--; 
      }

      let currentMaterial = materialAmber; 
      if (row >= 2 && row <= 4) currentMaterial = materialGreen; 
      if (row >= 5) currentMaterial = materialRoyalBlue; 

      for (let i = 0; i < seatsInRow; i++) {
        const centeredIndex = i - (seatsInRow - 1) / 2;
        const angle = (Math.PI / 2) + (centeredIndex * angleStep);

        const x = Math.cos(angle) * radius;
        const z = -Math.sin(angle) * radius;

        const seat = new THREE.Mesh(seatGeometry, currentMaterial);
        seat.position.set(x, height, z);
        seat.lookAt(0, height, 0); 
        
        seat.castShadow = true;
        seat.receiveShadow = true;
        this.scene.add(seat);
      }
    }
  }

  private onWindowResize = (): void => {
    const container = this.canvasContainer.nativeElement;
    this.camera.aspect = container.clientWidth / container.clientHeight;
    this.camera.updateProjectionMatrix();
    this.renderer.setSize(container.clientWidth, container.clientHeight);
    this.renderer.render(this.scene, this.camera);
  };
}